using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.PlantRates.Commands
{
    public class CreatePlantRatesCommand:IRequest<int>
    {
        public int DistrictRateId { get; set; }  // Link to DistrictRate

        // List of rates submitted as a matrix
        public List<CompensationRateEntryDto> Rates { get; set; } = new List<CompensationRateEntryDto>();
    }

    // Individual row in the matrix
    public class CompensationRateEntryDto
    {
        public int? Id { get; set; }  // ✅ Used to identify for update
        public int DistrictRateId { get; set; }
        public int? PlantId { get; set; } // Single plant
        public string? PlantType { get; set; } // "Crop" or "Tree"
        public int? GroupedPlantId { get; set; } // instead of guessing from list
        public List<int>? GroupedPlantIds { get; set; } // IDs for grouped plants
        public string? GroupName { get; set; } // Name of grouped plants
        public int GrowthStageId { get; set; } // Growth stage of the plant
        public int? CategoryId { get; set; } // Category e.g., Variety
        public int? CategoryInfoId { get; set; } // Specific Variety ID
        public int? CategoryInfoOption { get; set; } // Specific Variety ID
        public UnitOfMeasure Unit { get; set; } // Per Tree, Per Acre, etc.
        public decimal? Rate { get; set; } // Rate value
        public string? Assumptions { get; set; } // Any assumptions
        public string? DiscretionInfo { get; set; } // If no rate, provide guidance
        public string Quality { get; set; }
    }


    public class CreatePlantRateMatrixHandler:IRequestHandler<CreatePlantRatesCommand,int>
    {
        private readonly IRepository<PlantRate> _repository;
        private readonly IRepository<DistrictRate> _districtRepository;
        private readonly IRepository<Plant> _plantRepository;
        private readonly IRepository<PlantRateGroup> _plantRateGroupRepository;

        public CreatePlantRateMatrixHandler(
            IRepository<PlantRate> repository,
            IRepository<Plant> plantRepository,
            IRepository<DistrictRate> districtRepository,
            IRepository<PlantRateGroup> plantRateGroupRepository)
        {
            _repository = repository;
            _districtRepository = districtRepository;
            _plantRepository = plantRepository;
            _plantRateGroupRepository = plantRateGroupRepository;
        }

        public async Task<int> Handle(CreatePlantRatesCommand command,CancellationToken cancellationToken)
        {
            // Validate DistrictRate existence
            var districtRate = await _districtRepository.GetByIdAsync(command.DistrictRateId);
            if (districtRate == null)
                throw new Exception("DistrictRate not found.");

            if (!command.Rates.Any())
                throw new Exception("Rate matrix cannot be empty.");

            // Create CompensationRate records in bulk


            /* var plantRates = new List<PlantRate>();

             foreach (var plantRateDto in command.Rates)
             {
                 var plantRate = new PlantRate
                 {
                     DistrictRateId = districtRate.Id,
                     PlantId = plantRateDto.PlantId,
                     PlantType = plantRateDto.PlantType,
                     GroupName = plantRateDto.GroupName,
                     GrowthStageId = plantRateDto.GrowthStageId,
                     Unit = plantRateDto.Unit,
                     Rate = plantRateDto.Rate,
                     Assumptions = plantRateDto.Assumptions,
                     DiscretionInfo = plantRateDto.DiscretionInfo,
                     Status = ModerationStatus.Pending
                 };
                 // ✅ Assign category fields ONLY if provided
                 if (plantRateDto.CategoryId.HasValue)
                 {
                     plantRate.CategoryId = plantRateDto.CategoryId;
                     plantRate.CategoryInfoId = plantRateDto.CategoryInfoId;
                     plantRate.CategoryInfoOption = plantRateDto.CategoryInfoOption;
                 }
                 // ✅ Handle grouped plants (if provided)
                 if (plantRateDto.GroupedPlantId.HasValue)
                 {
                     plantRate.GroupedPlantId = plantRateDto.GroupedPlantId;
                 }
                 /*if (plantRateDto.GroupedPlantIds != null && plantRateDto.GroupedPlantIds.Any())
                 {
                     // We'll assume all grouped plant IDs belong to the same GroupedPlant.
                     // If multiple groupedPlantIds are ever allowed, handle differently.
                     //var groupId = plantRateDto.GroupedPlantIds.First(); // take first as GroupedPlantId
                     //plantRate.GroupedPlantId = groupId;

                     var groupedPlants = await _plantRepository.GetAll().AsQueryable()
                         .Where(p => plantRateDto.GroupedPlantIds.Contains(p.Id))
                         .ToListAsync(cancellationToken);

                     // ✅ Add each grouped plant to the junction table
                     foreach (var plant in groupedPlants)
                     {
                         var plantRateGroup = new PlantRateGroup
                         {
                             PlantRate = plantRate,
                             Plant = plant
                         };
                         _plantRateGroupRepository.Add(plantRateGroup);
                     }
                     // Clear PlantId if grouped
                     plantRate.PlantId = null;
                     plantRate.PlantType = null;
                 }



                 plantRates.Add(plantRate);
             }*/
            var upserts = new List<PlantRate>();

            foreach (var dto in command.Rates)
            {
                PlantRate? existing = null;

                if (dto.Id.HasValue)
                {
                    existing = await _repository.GetAll().AsQueryable()
                        .Include(x => x.PlantRateGroups)
                        .FirstOrDefaultAsync(x => x.Id == dto.Id.Value, cancellationToken);
                }

                if (existing == null)
                {
                    existing = new PlantRate();
                    _repository.AddWithoutSaving(existing);
                }

                // ✍️ Common properties
                existing.DistrictRateId = command.DistrictRateId;
                existing.PlantId = dto.PlantId;
                existing.PlantType = dto.PlantType;
                existing.GroupedPlantId = dto.GroupedPlantId;
                existing.GroupName = dto.GroupName;
                existing.GrowthStageId = dto.GrowthStageId;
                existing.CategoryId = dto.CategoryId;
                existing.CategoryInfoId = dto.CategoryInfoId;
                existing.CategoryInfoOption = dto.CategoryInfoOption;
                existing.Unit = dto.Unit;
                existing.Rate = dto.Rate;
                existing.Assumptions = dto.Assumptions;
                existing.DiscretionInfo = dto.DiscretionInfo;
                existing.Quality = dto.Quality;

                // 🔄 Remove & re-attach PlantRateGroups if group
                if (dto.GroupedPlantIds?.Any() == true)
                {
                    existing.PlantRateGroups.Clear();

                    foreach (var id in dto.GroupedPlantIds)
                    {
                        existing.PlantRateGroups.Add(new PlantRateGroup
                        {
                            PlantRate = existing,
                            PlantId = id
                        });
                    }
                }

                upserts.Add(existing);
            }
                // Use the async bulk insertion method
                return await _repository.AddManyAsync(upserts);
        }
    }
}
