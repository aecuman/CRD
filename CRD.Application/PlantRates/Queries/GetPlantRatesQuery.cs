using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CRD.Application.PlantRates.Queries
{
    public class PlantRateViewModel
    {
        public int Id { get; set; }
        public string DistrictName { get; set; }
        public int Year { get; set; }
        public int? PlantId { get; set; }
        public string? PlantName { get; set; }
        public string? PlantType { get; set; }

        // Grouped plant (optional)
        public int? GroupedPlantId { get; set; }
        public string? GroupName { get; set; }
        public List<GroupedPlantItemDto> Plants { get; set; } = new();
        public int? CategoryId { get; set; }
        public int? CategoryInfoId { get; set; } // Specific Variety ID
        public int? CategoryInfoOption { get; set; } // Specific Variety ID
        public string Category { get; set; }
        public int GrowthStage { get; set; }
        public string Unit { get; set; }
        public decimal? Rate { get; set; }
        public string Assumptions { get; set; }
        public string DiscretionInfo { get; set; }
        public string Quality { get; set; }
        public string Status { get; set; }
    }
    public class GetPlantsRatesQuery : IRequest<List<PlantRateViewModel>>
    {
        public int? DistrictRateId { get; set; } // Optional filter by district
        public int? PlantId { get; set; } // Optional filter by plant
    }
    public class GetCompensationRatesHandler : IRequestHandler<GetPlantsRatesQuery, List<PlantRateViewModel>>
    {
        private readonly IRepository<PlantRate> _context;

        public GetCompensationRatesHandler(IRepository<PlantRate> context)
        {
            _context = context;
        }

        public async Task<List<PlantRateViewModel>> Handle(GetPlantsRatesQuery request, CancellationToken cancellationToken)
        {
            IQueryable<PlantRate> query = _context.GetAll().AsQueryable()
      .Include(cr => cr.DistrictRate)
          .ThenInclude(dr => dr.District)
      .Include(cr => cr.CategoryInfo)
      .Include(cr => cr.Plant)
      .Include(cr => cr.GroupedPlants)
      .Include(cr => cr.PlantRateGroups)
          .ThenInclude(prg => prg.Plant);

            if (request.DistrictRateId.HasValue)
            {
                query  = query.Where(cr => cr.DistrictRateId == request.DistrictRateId);
            }

            if (request.PlantId.HasValue)
            {
                query = query.Where(cr =>
                    (cr.PlantId.HasValue && cr.PlantId == request.PlantId) ||
                    cr.PlantRateGroups.Any(gr => gr.PlantId == request.PlantId));
            }

            var rates = await query.Select(cr => new PlantRateViewModel
            {
                Id = cr.Id,
                DistrictName = cr.DistrictRate.District.Name,
                Year = cr.DistrictRate.Year,

                PlantId = cr.PlantId,
                PlantType = cr.PlantType,
                PlantName = cr.Plant != null
                    ? (cr.CategoryInfo != null ? cr.CategoryInfo.Info[cr.CategoryInfoOption.Value]+" - " : "") +(!string.IsNullOrWhiteSpace(cr.Plant.CommonName)
                        ? cr.Plant.CommonName
                        : cr.Plant.BotanicalName)
                    : null,

                GroupedPlantId = cr.GroupedPlantId,
                GroupName = cr.GroupedPlants != null ? cr.GroupedPlants.Name : cr.GroupName,

                Plants = cr.PlantRateGroups
                    .Select(prg => new GroupedPlantItemDto
                    {
                        Id = prg.PlantId,
                        Name = !string.IsNullOrWhiteSpace(prg.Plant.CommonName)
                            ? prg.Plant.CommonName
                            : prg.Plant.BotanicalName,
                        PlantType = prg.Plant.PlantType // assuming this property exists
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

            return rates;
        }
    }
 

    public class GroupedPlantItemDto
    {
        public int Id { get; set; }
        public string PlantType { get; set; } // "Crop" or "Tree"
        public string Name { get; set; }
    }
}
