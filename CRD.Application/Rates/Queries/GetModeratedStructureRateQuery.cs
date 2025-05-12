using CRD.Application.Common;
using CRD.Application.Common.Dtos;
using CRD.Application.StructureRates.Queries;
using CRD.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CRD.Application.Rates.Queries
{
    public class GetModeratedStructureRateQuery : IRequest<List<ModeratedStructureRateViewModel>>
    {
        public int? DistrictRateId { get; set; }
        public bool IncludeUnmoderated { get; set; } = false;
    }
    public class ModeratedStructureRateViewModel
    {
        public int Id { get; set; }
        public int StructureId { get; set; }
        public string StructureName { get; set; }
        public StructureRateViewDto Structure { get; set; }
        public string Unit { get; set; }
        public decimal? Rate { get; set; }
        public string DiscretionInfo { get; set; }
        public decimal? OriginalRate { get; set; }
        public string Status { get; set; }
        public bool IsModerated { get; set; }
        public ModerationDetailViewModel? LatestModeration { get; set; }
    }
    public class GetModeratedStructureRateHandler : IRequestHandler<GetModeratedStructureRateQuery, List<ModeratedStructureRateViewModel>>
    {
        private readonly IRepository<StructureRate> _context;

        public GetModeratedStructureRateHandler(IRepository<StructureRate> context)
        {
            _context = context;
        }

        public async Task<List<ModeratedStructureRateViewModel>> Handle(GetModeratedStructureRateQuery request, CancellationToken cancellationToken)
        {
            var query = _context.GetAll().AsQueryable()
            .Include(sr => sr.Structure).ThenInclude(s => s.Category)
            .Include(sr => sr.Structure).ThenInclude(s => s.StructureType)
            .Include(sr => sr.Structure).ThenInclude(s => s.AttributeSelections).ThenInclude(a => a.Attribute)
            .Include(sr => sr.Structure).ThenInclude(s => s.AttributeSelections).ThenInclude(a => a.OptionSelections).ThenInclude(o => o.Option)
            .Include(sr => sr.DistrictRate).ThenInclude(dr => dr.District).Include(sr => sr.ModerationHistory)
            .AsQueryable();

            if (request.DistrictRateId.HasValue)
            {
                query = query.Where(sr => sr.DistrictRateId == request.DistrictRateId.Value);
            }

            var structureRates = await query.ToListAsync(cancellationToken);

            var result = structureRates.Select(sr =>
            {
                var firstModeration = sr.ModerationHistory
                    .OrderBy(m => m.ModerationDate)
                    .FirstOrDefault();

                var latestModeration = sr.ModerationHistory
                    .OrderByDescending(m => m.ModerationDate)
                    .FirstOrDefault();

                return new ModeratedStructureRateViewModel
                {
                    Id = sr.Id,
                    StructureId = sr.StructureId,
                    StructureName = sr.Structure?.Name ?? "Unknown",
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
                    },
                    Unit = Regex.Replace(sr.Unit.ToString(), "(?<!^)([A-Z])", " $1"),
                    Rate = sr.Rate,
                    DiscretionInfo = sr.DiscretionInfo ?? string.Empty,
                    OriginalRate = firstModeration?.OldRate ?? sr.Rate,
                    Status = sr.Status.ToString(),
                    IsModerated = latestModeration != null,
                    LatestModeration = latestModeration != null
                        ? new ModerationDetailViewModel
                        {
                            OldRate = latestModeration.OldRate,
                            NewRate = latestModeration.NewRate,
                            ModerationNotes = latestModeration.ModerationNotes,
                            Moderator = latestModeration.Moderator,
                            ModerationDate = latestModeration.ModerationDate,
                            DiscretionaryInfo = latestModeration.DiscretionInfo,
                            DeferredReason = latestModeration.IsDeferred ? latestModeration.DeferredReason : null,
                            Status = latestModeration.Status.ToString()
                        }
                        : null
                };
            })
            .Where(r => request.IncludeUnmoderated || r.IsModerated)
            .ToList();

            return result;
        }
    }


}
