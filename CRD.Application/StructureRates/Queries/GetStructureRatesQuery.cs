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

namespace CRD.Application.StructureRates.Queries
{
    public class StructureRatesListViewModel
    {
        public int Id { get; set; }
        public string DistrictName { get; set; }
        public int StructureId { get; set; }
        public string StructureName { get; set; }
        public StructureRateViewDto Structure { get; set; }
        public int Year { get; set; }
        public string Unit { get; set; }
        public StructuresUnitOfMeasure UnitId { get; set; }
        public decimal? Rate { get; set; }
        public string Assumptions { get; set; }
        public string DiscretionInfo { get; set; }
        public string Status { get; set; }
    }

    public class GetStructureRatesQuery : IRequest<List<StructureRatesListViewModel>>
    {
        public int? DistrictRateId { get; set; }  // Optional filter
        public int? StructureId { get; set; }     // Optional filter
    }
    public class GetStructureRatesListHandler : IRequestHandler<GetStructureRatesQuery, List<StructureRatesListViewModel>>
    {
        private readonly IRepository<StructureRate> _context;

        public GetStructureRatesListHandler(IRepository<StructureRate> context)
        {
            _context = context;
        }

        public async Task<List<StructureRatesListViewModel>> Handle(GetStructureRatesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.GetAll().AsQueryable()
            .Include(sr => sr.Structure)
                .ThenInclude(s => s.Category)
            .Include(sr => sr.Structure)
                .ThenInclude(s => s.StructureType)
            .Include(sr => sr.Structure)
                .ThenInclude(s => s.AttributeSelections)
                    .ThenInclude(a => a.Attribute)
            .Include(sr => sr.Structure)
                .ThenInclude(s => s.AttributeSelections)
                    .ThenInclude(a => a.OptionSelections)
                        .ThenInclude(o => o.Option)
            .Include(sr => sr.DistrictRate)
                .ThenInclude(dr => dr.District)
            .AsQueryable();

            if (request.DistrictRateId.HasValue)
                query = query.Where(sr => sr.DistrictRateId == request.DistrictRateId.Value);

            if (request.StructureId.HasValue)
                query = query.Where(sr => sr.StructureId == request.StructureId.Value);

            var result = await query.Select(sr => new StructureRatesListViewModel
            {
                Id = sr.Id,
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

            return result;
        }

    }
    public class StructureRateViewDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public StructureRateCategoryDto Category { get; set; }
        public StructureRateTypeDto StructureType { get; set; }
        public List<StructureRateAttributeSelectionDto> AttributeSelections { get; set; } = new List<StructureRateAttributeSelectionDto>();
    }

    public class StructureRateTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class StructureRateCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class StructureRateAttributeSelectionDto
    {
        public int AttributeId { get; set; }
        public string AttributeName { get; set; }
        public List<StructureRateOptionDto> SelectedOptions { get; set; } = new List<StructureRateOptionDto>();
    }

    public class StructureRateOptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }


}
