using CRD.Application.Common;
using CRD.Application.Common.Exceptions;
using CRD.Application.PlantRates.Queries;
using CRD.Application.StructureRates.Commands;
using CRD.Application.StructureRates.Queries;
using CRD.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CRD.Application.Districts.Queries
{
    public class GetComparableDistrictRateDetailByIdQuery : IRequest<ComparableDistrictRateDetailDto>
    {
        public int Id { get; set; }
        public string Context { get; set; } = "both"; // Accepts: "plant", "structure", or "both"
    }
    public class ComparableDistrictRateDetailDto
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public string DistrictName { get; set; } = default!;
        public List<int> ComparableDistrictRatesIds { get; set; } = new();
        public List<PlantRateViewModel> PlantRates { get; set; } = new();
        public List<StructureRatesListViewModel> StructureRates { get; set; } = new();
    }
    public class GetComparableDistrictRateDetailByIdHandler : IRequestHandler<GetComparableDistrictRateDetailByIdQuery, ComparableDistrictRateDetailDto>
    {
        private readonly IRepository<DistrictRate> _context;
        private readonly IRepository<PlantRate> _plantRateContext;
        private readonly IRepository<StructureRate> _structureContext;

        public GetComparableDistrictRateDetailByIdHandler(IRepository<DistrictRate> context, IRepository<PlantRate> plantRateContext, IRepository<StructureRate> structureContext)
        {
            _context = context;
            _plantRateContext = plantRateContext;
            _structureContext = structureContext;
        }

        public async Task<ComparableDistrictRateDetailDto> Handle(GetComparableDistrictRateDetailByIdQuery request, CancellationToken cancellationToken)
        {
            var baseDistrictRate = await _context.GetAll().AsQueryable()
            .Include(dr => dr.District)
            .AsNoTracking()
            .FirstOrDefaultAsync(dr => dr.Id == request.Id, cancellationToken);

            if (baseDistrictRate == null)
                throw new NotFoundException(nameof(DistrictRate), request.Id);

            var plantRates = new List<PlantRateViewModel>();
            var structureRates = new List<StructureRatesListViewModel>();

            foreach (var comparableId in baseDistrictRate.ComparableDistrictRatesIds ?? new List<int>())
            {
                if (request.Context == "plant" || request.Context == "both")
                {
                    // --- Plant Rates for Comparable District ---
                    var compPlantRates = await _plantRateContext.GetAll().AsQueryable()
                    .Include(cr => cr.DistrictRate).ThenInclude(dr => dr.District)
                    .Include(cr => cr.CategoryInfo)
                    .Include(cr => cr.Plant)
                    .Include(cr => cr.GroupedPlants)
                    .Include(cr => cr.PlantRateGroups).ThenInclude(prg => prg.Plant)
                    .Where(cr => cr.DistrictRateId == comparableId)
                    .Select(cr => new PlantRateViewModel
                    {
                        Id = cr.Id,
                        // DistrictRateId = cr.DistrictRateId,
                        DistrictName = cr.DistrictRate.District.Name,
                        Year = cr.DistrictRate.Year,
                        PlantId = cr.PlantId,
                        PlantType = cr.PlantType,
                        PlantName = cr.Plant != null
                            ? (cr.CategoryInfo != null ? cr.CategoryInfo.Info[cr.CategoryInfoOption.Value] + " - " : "") +
                                (!string.IsNullOrWhiteSpace(cr.Plant.CommonName) ? cr.Plant.CommonName : cr.Plant.BotanicalName)
                            : null,
                        GroupedPlantId = cr.GroupedPlantId,
                        GroupName = cr.GroupedPlants != null ? cr.GroupedPlants.Name : cr.GroupName,
                        Plants = cr.PlantRateGroups.Select(prg => new GroupedPlantItemDto
                        {
                            Id = prg.PlantId,
                            Name = !string.IsNullOrWhiteSpace(prg.Plant.CommonName) ? prg.Plant.CommonName : prg.Plant.BotanicalName,
                            PlantType = prg.Plant.PlantType
                        }).ToList(),
                        Category = cr.CategoryInfo != null ? cr.CategoryInfo.Info[cr.CategoryInfoOption.Value] : "N/A",
                        CategoryId = cr.CategoryId,
                        CategoryInfoId = cr.CategoryInfo != null ? cr.CategoryInfo.Id : null,
                        CategoryInfoOption = cr.CategoryInfoOption,
                        GrowthStage = cr.GrowthStageId,
                        Unit = Regex.Replace(cr.Unit.ToString(), "(?<!^)([A-Z])", " $1"),
                        Rate = cr.Rate,
                        Assumptions = cr.Assumptions,
                        DiscretionInfo = cr.DiscretionInfo,
                        Quality = cr.Quality,
                        Status = cr.Status.ToString()
                    }).ToListAsync(cancellationToken);

                    plantRates.AddRange(compPlantRates);
                }
                if (request.Context == "structure" || request.Context == "both")
                {

                    // --- Structure Rates for Comparable District ---
                    var compStructureRates = await _structureContext.GetAll().AsQueryable()
                    .Include(sr => sr.Structure).ThenInclude(s => s.Category)
                    .Include(sr => sr.Structure).ThenInclude(s => s.StructureType)
                    .Include(sr => sr.Structure).ThenInclude(s => s.AttributeSelections)
                        .ThenInclude(a => a.Attribute)
                    .Include(sr => sr.Structure).ThenInclude(s => s.AttributeSelections)
                        .ThenInclude(a => a.OptionSelections).ThenInclude(o => o.Option)
                    .Include(sr => sr.DistrictRate).ThenInclude(dr => dr.District)
                    .Where(sr => sr.DistrictRateId == comparableId)
                    .Select(sr => new StructureRatesListViewModel
                    {
                        Id = sr.Id,
                        // DistrictRateId = sr.DistrictRateId,
                        DistrictName = sr.DistrictRate.District.Name,
                        StructureId = sr.StructureId,
                        StructureName = sr.Structure.Name,
                        Year = sr.DistrictRate.Year,
                        Unit = Regex.Replace(sr.Unit.ToString(), "(?<!^)([A-Z])", " $1"),
                        UnitId = sr.Unit,
                        Rate = sr.Rate,
                        Assumptions = sr.Assumptions,
                        DiscretionInfo = sr.DiscretionInfo,
                        Status = sr.Status.ToString(),
                        Structure = new StructureRateViewDto
                        {
                            Id = sr.Structure.Id,
                            Name = sr.Structure.Name,
                            Category = new StructureRateCategoryDto
                            {
                                Id = sr.Structure.Category.Id,
                                Name = sr.Structure.Category.Name
                            },
                            StructureType = new StructureRateTypeDto
                            {
                                Id = sr.Structure.StructureType.Id,
                                Name = sr.Structure.StructureType.Name
                            },
                            AttributeSelections = sr.Structure.AttributeSelections.Select(a => new StructureRateAttributeSelectionDto
                            {
                                AttributeId = a.Attribute.Id,
                                AttributeName = a.Attribute.Name,
                                SelectedOptions = a.OptionSelections.Select(o => new StructureRateOptionDto
                                {
                                    Id = o.Option.Id,
                                    Name = o.Option.Name
                                }).ToList()
                            }).ToList()
                        }
                    }).ToListAsync(cancellationToken);

                    structureRates.AddRange(compStructureRates);
                }
            }

            return new ComparableDistrictRateDetailDto
            {
                Id = baseDistrictRate.Id,
                Year = baseDistrictRate.Year,
                DistrictName = baseDistrictRate.District.Name,
                ComparableDistrictRatesIds = baseDistrictRate.ComparableDistrictRatesIds ?? new List<int>(),
                PlantRates = plantRates,
                StructureRates = structureRates
            };
            /* var districtRate = await _context.GetAll().AsQueryable()
                 .Include(dr => dr.District)
                 .AsNoTracking()
                 .FirstOrDefaultAsync(dr => dr.Id == request.Id, cancellationToken);

             if (districtRate == null)
                 throw new NotFoundException(nameof(DistrictRate), request.Id);

             var plantRates = new List<PlantRateViewModel>();
             var structureRates = new List<StructureRatesListViewModel>();

             // ---------------- Load current PlantRates ----------------
             var basePlantRates = await _plantRateContext.GetAll().AsQueryable()
                 .Include(cr => cr.DistrictRate).ThenInclude(dr => dr.District)
                 .Include(cr => cr.CategoryInfo)
                 .Include(cr => cr.Plant)
                 .Include(cr => cr.GroupedPlants)
                 .Include(cr => cr.PlantRateGroups).ThenInclude(prg => prg.Plant)
                 .Where(cr => cr.DistrictRateId == request.Id)
                 .Select(cr => new PlantRateViewModel
                 {
                     Id = cr.Id,
                    // DistrictRateId = cr.DistrictRateId,
                     DistrictName = cr.DistrictRate.District.Name,
                     Year = cr.DistrictRate.Year,
                     PlantId = cr.PlantId,
                     PlantType = cr.PlantType,
                     PlantName = cr.Plant != null
                         ? (cr.CategoryInfo != null ? cr.CategoryInfo.Info[cr.CategoryInfoOption.Value] + " - " : "") +
                             (!string.IsNullOrWhiteSpace(cr.Plant.CommonName) ? cr.Plant.CommonName : cr.Plant.BotanicalName)
                         : null,
                     GroupedPlantId = cr.GroupedPlantId,
                     GroupName = cr.GroupedPlants != null ? cr.GroupedPlants.Name : cr.GroupName,
                     Plants = cr.PlantRateGroups.Select(prg => new GroupedPlantItemDto
                     {
                         Id = prg.PlantId,
                         Name = !string.IsNullOrWhiteSpace(prg.Plant.CommonName) ? prg.Plant.CommonName : prg.Plant.BotanicalName,
                         PlantType = prg.Plant.PlantType
                     }).ToList(),
                     Category = cr.CategoryInfo != null ? cr.CategoryInfo.Info[cr.CategoryInfoOption.Value] : "N/A",
                     CategoryId = cr.CategoryId,
                     CategoryInfoId = cr.CategoryInfo != null ? cr.CategoryInfo.Id : null,
                     CategoryInfoOption = cr.CategoryInfoOption,
                     GrowthStage = cr.GrowthStageId,
                     Unit = Regex.Replace(cr.Unit.ToString(), "(?<!^)([A-Z])", " $1"),
                     Rate = cr.Rate,
                     Assumptions = cr.Assumptions,
                     DiscretionInfo = cr.DiscretionInfo,
                     Quality = cr.Quality,
                     Status = cr.Status.ToString()
                 }).ToListAsync(cancellationToken);

             plantRates.AddRange(basePlantRates);

             // ---------------- Load current StructureRates ----------------
             var baseStructureRates = await _structureContext.GetAll().AsQueryable()
                 .Include(sr => sr.Structure).ThenInclude(s => s.Category)
                 .Include(sr => sr.Structure).ThenInclude(s => s.StructureType)
                 .Include(sr => sr.Structure).ThenInclude(s => s.AttributeSelections)
                     .ThenInclude(a => a.Attribute)
                 .Include(sr => sr.Structure).ThenInclude(s => s.AttributeSelections)
                     .ThenInclude(a => a.OptionSelections).ThenInclude(o => o.Option)
                 .Include(sr => sr.DistrictRate).ThenInclude(dr => dr.District)
                 .Where(sr => sr.DistrictRateId == request.Id)
                 .Select(sr => new StructureRatesListViewModel
                 {
                     Id = sr.Id,
                 //    DistrictRateId = sr.DistrictRateId,
                     DistrictName = sr.DistrictRate.District.Name,
                     StructureId = sr.StructureId,
                     StructureName = sr.Structure.Name,
                     Year = sr.DistrictRate.Year,
                     Unit = Regex.Replace(sr.Unit.ToString(), "(?<!^)([A-Z])", " $1"),
                     UnitId = sr.Unit,
                     Rate = sr.Rate,
                     Assumptions = sr.Assumptions,
                     DiscretionInfo = sr.DiscretionInfo,
                     Status = sr.Status.ToString(),
                     Structure = new StructureRateViewDto
                     {
                         Id = sr.Structure.Id,
                         Name = sr.Structure.Name,
                         Category = new StructureRateCategoryDto
                         {
                             Id = sr.Structure.Category.Id,
                             Name = sr.Structure.Category.Name
                         },
                         StructureType = new StructureRateTypeDto
                         {
                             Id = sr.Structure.StructureType.Id,
                             Name = sr.Structure.StructureType.Name
                         },
                         AttributeSelections = sr.Structure.AttributeSelections.Select(a => new StructureRateAttributeSelectionDto
                         {
                             AttributeId = a.Attribute.Id,
                             AttributeName = a.Attribute.Name,
                             SelectedOptions = a.OptionSelections.Select(o => new StructureRateOptionDto
                             {
                                 Id = o.Option.Id,
                                 Name = o.Option.Name
                             }).ToList()
                         }).ToList()
                     }
                 }).ToListAsync(cancellationToken);

             structureRates.AddRange(baseStructureRates);

             // ---------------- Load all comparable rates ----------------
             foreach (var comparableId in districtRate.ComparableDistrictRatesIds ?? new List<int>())
             {
                 // Plant rates
                 var compPlantRates = await _context.PlantRates
                     .Include(cr => cr.DistrictRate).ThenInclude(dr => dr.District)
                     .Include(cr => cr.CategoryInfo)
                     .Include(cr => cr.Plant)
                     .Include(cr => cr.GroupedPlants)
                     .Include(cr => cr.PlantRateGroups).ThenInclude(prg => prg.Plant)
                     .Where(cr => cr.DistrictRateId == comparableId)
                     .Select(cr => new PlantRateViewModel
                     {
                         Id = cr.Id,
                         DistrictRateId = cr.DistrictRateId,
                         DistrictName = cr.DistrictRate.District.Name,
                         Year = cr.DistrictRate.Year,
                         PlantId = cr.PlantId,
                         PlantType = cr.PlantType,
                         PlantName = cr.Plant != null
                             ? (cr.CategoryInfo != null ? cr.CategoryInfo.Info[cr.CategoryInfoOption.Value] + " - " : "") +
                                 (!string.IsNullOrWhiteSpace(cr.Plant.CommonName) ? cr.Plant.CommonName : cr.Plant.BotanicalName)
                             : null,
                         GroupedPlantId = cr.GroupedPlantId,
                         GroupName = cr.GroupedPlants?.Name ?? cr.GroupName,
                         Plants = cr.PlantRateGroups.Select(prg => new GroupedPlantItemDto
                         {
                             Id = prg.PlantId,
                             Name = !string.IsNullOrWhiteSpace(prg.Plant.CommonName) ? prg.Plant.CommonName : prg.Plant.BotanicalName,
                             PlantType = prg.Plant.PlantType
                         }).ToList(),
                         Category = cr.CategoryInfo != null ? cr.CategoryInfo.Info[cr.CategoryInfoOption.Value] : "N/A",
                         CategoryId = cr.CategoryId,
                         CategoryInfoId = cr.CategoryInfo?.Id,
                         CategoryInfoOption = cr.CategoryInfoOption,
                         GrowthStage = cr.GrowthStageId,
                         Unit = Regex.Replace(cr.Unit.ToString(), "(?<!^)([A-Z])", " $1"),
                         Rate = cr.Rate,
                         Assumptions = cr.Assumptions,
                         DiscretionInfo = cr.DiscretionInfo,
                         Quality = cr.Quality,
                         Status = cr.Status.ToString()
                     }).ToListAsync(cancellationToken);

                 plantRates.AddRange(compPlantRates);

                 // Structure rates
                 var compStructureRates = await _context.StructureRates
                     .Include(sr => sr.Structure).ThenInclude(s => s.Category)
                     .Include(sr => sr.Structure).ThenInclude(s => s.StructureType)
                     .Include(sr => sr.Structure).ThenInclude(s => s.AttributeSelections)
                         .ThenInclude(a => a.Attribute)
                     .Include(sr => sr.Structure).ThenInclude(s => s.AttributeSelections)
                         .ThenInclude(a => a.OptionSelections).ThenInclude(o => o.Option)
                     .Include(sr => sr.DistrictRate).ThenInclude(dr => dr.District)
                     .Where(sr => sr.DistrictRateId == comparableId)
                     .Select(sr => new StructureRatesListViewModel
                     {
                         Id = sr.Id,
                         DistrictRateId = sr.DistrictRateId,
                         DistrictName = sr.DistrictRate.District.Name,
                         StructureId = sr.StructureId,
                         StructureName = sr.Structure.Name,
                         Year = sr.DistrictRate.Year,
                         Unit = Regex.Replace(sr.Unit.ToString(), "(?<!^)([A-Z])", " $1"),
                         UnitId = sr.Unit,
                         Rate = sr.Rate,
                         Assumptions = sr.Assumptions,
                         DiscretionInfo = sr.DiscretionInfo,
                         Status = sr.Status.ToString(),
                         Structure = new StructureRateViewDto
                         {
                             Id = sr.Structure.Id,
                             Name = sr.Structure.Name,
                             Category = new StructureRateCategoryDto
                             {
                                 Id = sr.Structure.Category.Id,
                                 Name = sr.Structure.Category.Name
                             },
                             StructureType = new StructureRateTypeDto
                             {
                                 Id = sr.Structure.StructureType.Id,
                                 Name = sr.Structure.StructureType.Name
                             },
                             AttributeSelections = sr.Structure.AttributeSelections.Select(a => new StructureRateAttributeSelectionDto
                             {
                                 AttributeId = a.Attribute.Id,
                                 AttributeName = a.Attribute.Name,
                                 SelectedOptions = a.OptionSelections.Select(o => new StructureRateOptionDto
                                 {
                                     Id = o.Option.Id,
                                     Name = o.Option.Name
                                 }).ToList()
                             }).ToList()
                         }
                     }).ToListAsync(cancellationToken);

                 structureRates.AddRange(compStructureRates);
             }

             return new ComparableDistrictRateDetailDto
             {
                 Id = districtRate.Id,
                 Year = districtRate.Year,
                 DistrictName = districtRate.District.Name,
                 ComparableDistrictRatesIds = districtRate.ComparableDistrictRatesIds ?? new List<int>(),
                 PlantRates = plantRates,
                 StructureRates = structureRates
             };
         }*/
        }
    }

    }
