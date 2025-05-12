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

namespace CRD.Application.Rates.Queries
{
    public class ModeratedPlantRateViewModel
    {
        public int Id { get; set; }
        // Grouped plant (optional)
        public int? GroupedPlantId { get; set; }
        public string? GroupName { get; set; }
        public int? PlantId { get; set; }
        public string PlantName { get; set; }
        public string CategoryName { get; set; }
        public int GrowthStage { get; set; }
        public string Unit { get; set; }
        public string Quality { get; set; }
        public decimal? Rate { get; set; }
        public decimal? OriginalRate { get; set; }
        public string Status { get; set; }
        public bool IsModerated { get; set; } 

        public ModerationDetailViewModel? LatestModeration { get; set; }
    }
    public class ModerationDetailViewModel
    {
        public decimal? OldRate { get; set; }
        public decimal? NewRate { get; set; }
        public string ModerationNotes { get; set; }
        public string Moderator { get; set; }
        public DateTime ModerationDate { get; set; }
        public string? DiscretionaryInfo { get; set; }
        public string? DeferredReason { get; set; }
        public string Status { get; set; }
    }
    public class GetModeratedPlantRateQuery : IRequest<List<ModeratedPlantRateViewModel>>
    {
        public int? DistrictRateId { get; set; } // Optional filter
        public bool IncludeUnmoderated { get; set; } = false;
    }
    public class GetModeratedPlantRateHandler : IRequestHandler<GetModeratedPlantRateQuery, List<ModeratedPlantRateViewModel>>
    {
        private readonly IRepository<PlantRate> _context;


        public GetModeratedPlantRateHandler(IRepository<PlantRate> context)
        {
            _context = context;
        }

        public async Task<List<ModeratedPlantRateViewModel>> Handle(GetModeratedPlantRateQuery request, CancellationToken cancellationToken)
        {
            var query = await _context.GetAll().AsQueryable()
                .Include(p => p.Plant)
        .Include(p => p.CategoryInfo)
        .Include(p => p.ModerationHistory)
        .Where(p => !p.IsDeleted &&
               (!request.DistrictRateId.HasValue || p.DistrictRateId == request.DistrictRateId.Value))
        .ToListAsync(cancellationToken);
            /*.Include(cr => cr.DistrictRate)
      .ThenInclude(dr => dr.District)
  .Include(cr => cr.CategoryInfo)
            .Include(p => p.ModerationHistory)
  .Include(cr => cr.Plant)
  .Include(cr => cr.GroupedPlants)
  .Include(cr => cr.PlantRateGroups)
      .ThenInclude(prg => prg.Plant).Where(p => !p.IsDeleted &&
            (!request.DistrictRateId.HasValue || p.DistrictRateId == request.DistrictRateId.Value)).ToListAsync();*/

            var result = query
.Select(pr =>
{
    var latestModeration = pr.ModerationHistory
        .OrderByDescending(m => m.ModerationDate)
        .FirstOrDefault();

    return new ModeratedPlantRateViewModel
    {
        Id = pr.Id,
        GroupedPlantId = pr.GroupedPlantId,
        GroupName = pr.GroupName ?? "",
        PlantId = pr.PlantId,
        PlantName = pr.Plant != null
            ? (!string.IsNullOrWhiteSpace(pr.Plant.CommonName) ? pr.Plant.CommonName : pr.Plant.BotanicalName)
            : "(Grouped)",
        CategoryName = pr.CategoryInfo != null && pr.CategoryInfoOption.HasValue
            ? pr.CategoryInfo.Info.ElementAtOrDefault(pr.CategoryInfoOption.Value) ?? "N/A"
            : "N/A",
        GrowthStage = pr.GrowthStageId,
        Unit = Regex.Replace(pr.Unit.ToString(), "(?<!^)([A-Z])", " $1"),
        Quality = pr.Quality ?? "Good",
        Rate = pr.Rate,
        OriginalRate=pr.ModerationHistory
    .OrderBy(m => m.ModerationDate)
    .Select(m => m.OldRate)
    .FirstOrDefault() ?? pr.Rate ,
        Status = pr.Status.ToString(),
        IsModerated = latestModeration != null,
        LatestModeration = latestModeration != null ? new ModerationDetailViewModel
        {
            OldRate = latestModeration.OldRate,
            NewRate = latestModeration.NewRate,
            ModerationNotes = latestModeration.ModerationNotes,
            Moderator = latestModeration.Moderator,
            ModerationDate = latestModeration.ModerationDate,
            DiscretionaryInfo = latestModeration.DiscretionInfo,
            DeferredReason = latestModeration.IsDeferred ? latestModeration.DeferredReason : null,
            Status = latestModeration.Status.ToString()
        } : null
    };
})
.Where(r => request.IncludeUnmoderated || r.LatestModeration != null)
.ToList();
            return result;


            /*   var result = await query
                   .Select(pr => new ModeratedPlantRateViewModel
                   {
                       Id = pr.Id,
                       GroupedPlantId = pr.GroupedPlantId,
                       GroupName = pr.GroupName ?? "",
                       PlantId = pr.PlantId,
                       PlantName = pr.Plant != null
                           ? (!string.IsNullOrWhiteSpace(pr.Plant.CommonName) ? pr.Plant.CommonName : pr.Plant.BotanicalName)
                           : "(Grouped)",
                       CategoryName = pr.CategoryInfo != null && pr.CategoryInfoOption.HasValue
                           ? pr.CategoryInfo.Info.ElementAtOrDefault(pr.CategoryInfoOption.Value) ?? "N/A"
                           : "N/A",
                       GrowthStage = pr.GrowthStageId,
                       Unit = pr.Unit.ToString(),
                       Quality = pr.Quality ?? "Good",
                       Rate = pr.Rate,
                       Status = pr.Status.ToString(),

                       LatestModeration = pr.ModerationHistory
                           .OrderByDescending(m => m.ModerationDate)
                           .Select(m => new ModerationDetailViewModel
                           {
                               OldRate = m.OldRate,
                               NewRate = m.NewRate,
                               ModerationNotes = m.ModerationNotes,
                               Moderator = m.Moderator,
                               ModerationDate = m.ModerationDate,
                               DiscretionaryInfo = m.DiscretionInfo,
                               DeferredReason = m.IsDeferred ? m.DeferredReason : null,
                               Status = m.Status.ToString()
                           })
                           .FirstOrDefault()
                   })
                   .Where(r => r.LatestModeration != null)
                   .ToListAsync(cancellationToken);

               return result;*/
        }
    }
    public static class EnumExtensions
    {
        public static TEnum ParseEnumSafe<TEnum>(this string? value, TEnum defaultValue = default) where TEnum : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            return Enum.TryParse<TEnum>(value, true, out var result) ? result : defaultValue;
        }
    }

}
