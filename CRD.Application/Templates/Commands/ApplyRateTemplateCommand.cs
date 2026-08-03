using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
namespace CRD.Application.Templates.Commands
{
    /// <summary>
    /// Applies a saved template to a district rate — pre-creates all rate rows (plant + structure)
    /// with null rates so the data entry clerk only needs to fill in numbers.
    /// Existing rows for the same plant/group are skipped (not duplicated).
    /// Grouped plants are fully supported via PlantRateGroup junction records.
    /// </summary>
    public class ApplyRateTemplateCommand : IRequest<ApplyRateTemplateResult>
    {
        public int TemplateId { get; set; }
        public int DistrictRateId { get; set; }
    }

    public class ApplyRateTemplateResult
    {
        public int PlantRowsCreated { get; set; }
        public int StructureRowsCreated { get; set; }
        public int PlantRowsSkipped { get; set; }
        public int StructureRowsSkipped { get; set; }
    }

    public class ApplyRateTemplateHandler : IRequestHandler<ApplyRateTemplateCommand, ApplyRateTemplateResult>
    {
        private readonly IRepository<RateTemplate> _templateRepo;
        private readonly IRepository<DistrictRate> _districtRateRepo;
        private readonly IRepository<PlantRate> _plantRateRepo;
        private readonly IRepository<PlantRateGroup> _plantRateGroupRepo;
        private readonly IRepository<StructureRate> _structureRateRepo;

        public ApplyRateTemplateHandler(
            IRepository<RateTemplate> templateRepo,
            IRepository<DistrictRate> districtRateRepo,
            IRepository<PlantRate> plantRateRepo,
            IRepository<PlantRateGroup> plantRateGroupRepo,
            IRepository<StructureRate> structureRateRepo)
        {
            _templateRepo = templateRepo;
            _districtRateRepo = districtRateRepo;
            _plantRateRepo = plantRateRepo;
            _plantRateGroupRepo = plantRateGroupRepo;
            _structureRateRepo = structureRateRepo;
        }

        public async Task<ApplyRateTemplateResult> Handle(ApplyRateTemplateCommand command, CancellationToken cancellationToken)
        {
            var template = await _templateRepo.GetByIdAsync(command.TemplateId)
                ?? throw new Exception($"Template {command.TemplateId} not found.");

            var districtRate = await _districtRateRepo.GetByIdAsync(command.DistrictRateId)
                ?? throw new Exception($"DistrictRate {command.DistrictRateId} not found.");

            var config = JsonSerializer.Deserialize<RateTemplateConfig>(template.ConfigJson)
                ?? throw new Exception("Template config is invalid.");

            var result = new ApplyRateTemplateResult();

            // ── 1. Existing plant rate keys for dedup ────────────────────────────
            var existingPlantRates = _plantRateRepo.GetAll()
                .Where(r => r.DistrictRateId == command.DistrictRateId)
                .ToList();

            var newPlantRates = new List<PlantRate>();

            foreach (var item in config.PlantItems)
            {
                foreach (var unit in item.Units)
                {
                    foreach (var stageId in item.GrowthStageIds)
                    {
                        foreach (var quality in item.Qualities)
                        {
                            // Category info: one row per selected category info option
                            var categoryInfoList = item.CategoryInfos?.Any() == true
                                ? item.CategoryInfos
                                : new List<TemplateCategoryInfoSelection> { null! };

                            foreach (var catInfo in categoryInfoList)
                            {
                                // Dedup: skip if a rate already exists for this exact combination
                                bool alreadyExists = item.GroupedPlantId.HasValue
                                    ? existingPlantRates.Any(r =>
                                        r.GroupedPlantId == item.GroupedPlantId &&
                                        r.GrowthStageId == stageId &&
                                        (int)r.Unit == unit &&
                                        r.Quality == quality &&
                                        r.CategoryInfoOption == (catInfo != null ? (int?)catInfo.CategoryInfoOption : null))
                                    : existingPlantRates.Any(r =>
                                        r.PlantId == item.PlantId &&
                                        r.GrowthStageId == stageId &&
                                        (int)r.Unit == unit &&
                                        r.Quality == quality &&
                                        r.CategoryInfoOption == (catInfo != null ? (int?)catInfo.CategoryInfoOption : null));

                                if (alreadyExists)
                                {
                                    result.PlantRowsSkipped++;
                                    continue;
                                }

                                var row = new PlantRate
                                {
                                    DistrictRateId = command.DistrictRateId,
                                    GrowthStageId = stageId,
                                    Unit = (UnitOfMeasure)unit,
                                    Quality = quality,
                                    Rate = null,
                                    Status = ModerationStatus.Pending
                                };

                                if (catInfo != null)
                                {
                                    row.CategoryId = catInfo.CategoryId;
                                    row.CategoryInfoId = catInfo.CategoryInfoId;
                                    row.CategoryInfoOption = catInfo.CategoryInfoOption;
                                }

                                if (item.GroupedPlantId.HasValue)
                                {
                                    // Grouped plant
                                    row.GroupedPlantId = item.GroupedPlantId;
                                    row.GroupName = item.GroupName;

                                    if (item.GroupedPlantIds?.Any() == true)
                                    {
                                        foreach (var pid in item.GroupedPlantIds)
                                        {
                                            row.PlantRateGroups.Add(new PlantRateGroup { PlantRate = row, PlantId = pid });
                                        }
                                    }
                                }
                                else
                                {
                                    // Individual plant
                                    row.PlantId = item.PlantId;
                                    row.PlantType = item.PlantType;
                                }

                                newPlantRates.Add(row);
                                result.PlantRowsCreated++;
                            }
                        }
                    }
                }
            }

            if (newPlantRates.Any())
                await _plantRateRepo.AddManyAsync(newPlantRates);

            // ── 2. Structure rates ──────────────────────────────────────────────
            var existingStructureRates = _structureRateRepo.GetAll()
                .Where(r => r.DistrictRateId == command.DistrictRateId)
                .ToList();

            var newStructureRates = new List<StructureRate>();

            foreach (var item in config.StructureItems)
            {
                bool alreadyExists = existingStructureRates.Any(r =>
                    r.StructureId == item.StructureId && (int)r.Unit == item.Unit);

                if (alreadyExists)
                {
                    result.StructureRowsSkipped++;
                    continue;
                }

                newStructureRates.Add(new StructureRate
                {
                    DistrictRateId = command.DistrictRateId,
                    StructureId = item.StructureId,
                    Unit = (StructuresUnitOfMeasure)item.Unit,
                    Rate = null,
                    Status = ModerationStatus.Pending
                });
                result.StructureRowsCreated++;
            }

            if (newStructureRates.Any())
                await _structureRateRepo.AddManyAsync(newStructureRates);

            return result;
        }
    }
}
