using CRD.Application.Common;
using CRD.Domain.Entities;
using CRD.Domain.Process;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CRD.Application.Rates.Queries
{

        public class GetPublishedRatesQuery : IRequest<List<PublishedRateSummaryDto>>
        {
            public string? Filter { get; set; } // "all", "valid", "expired", "notPublished"
        }
    public class PublishedRateSummaryDto
    {
        public int DistrictId { get; set; }
        public int? DistrictRateId { get; set; }
        public string DistrictName { get; set; }
        public int? Year { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsExpired => (Year.HasValue ? Year.Value + 1 : DateTime.UtcNow.Year) < DateTime.UtcNow.Year;

        public WorkflowStatus? CurrentWorkflowStatus { get; set; }
        public string? CurrentWorkflowStatusName => CurrentWorkflowStatus?.ToString() ?? "Not in Workflow";

        public string Status => IsExpired ? "Expired":  Year.HasValue ? "Published": InWorkflowProcess? "Under Review"  : "Not Published";
        public bool InWorkflowProcess { get; set; }
        
    }

    public class GetPublishedRatesHandler : IRequestHandler<GetPublishedRatesQuery, List<PublishedRateSummaryDto>>
    {
        private readonly IRepository<District> _districtRepo;
        private readonly IRepository<DistrictWorkflow> _workflowRepo;
        private readonly IPublishedRateMongoService _mongoService;
        private readonly IRepository<DistrictRate> _districtRatecontext;

        public GetPublishedRatesHandler(
            IRepository<District> districtRepo,
            IRepository<DistrictWorkflow> workflowRepo,
            IPublishedRateMongoService mongoService,
            IRepository<DistrictRate> districtRatecontext)
        {
            _districtRepo = districtRepo;
            _workflowRepo = workflowRepo;
            _mongoService = mongoService;
            _districtRatecontext = districtRatecontext;
        }

        public async Task<List<PublishedRateSummaryDto>> Handle(GetPublishedRatesQuery request, CancellationToken cancellationToken)
        {
            var allDistricts = await _districtRepo.GetAll().AsQueryable().ToListAsync(cancellationToken);

            var query = _districtRatecontext.GetAll().AsQueryable()
                .Include(dr => dr.District).Where(x=>!(x.Status==DistrictRateStatus.Published))
                //.Include(dr => dr.Uploads) // Fetch the related files
                .AsQueryable();

            // Fetch the latest published rate per district from Mongo
            var allPublished = await _mongoService.GetLatestByDistrictAsync(cancellationToken);

            var publishedMap = allPublished.ToDictionary(p => p.DistrictId, p => p);

            // Get workflow states for each district still in progress
            var workflowMap = await _workflowRepo.GetAll().AsQueryable()
                .Where(w => w.Status != WorkflowStatus.Published)
                .GroupBy(w => w.DistrictId)
                .Select(g => new { DistrictId = g.Key, Status = g.Max(w => w.Status) })
                .ToDictionaryAsync(g => g.DistrictId, g => g.Status, cancellationToken);

            // Compile results
            var results = allDistricts.Select(d =>
            {
                var published = publishedMap.GetValueOrDefault(d.Id);
                var inWorkflow = query.FirstOrDefault(x => x.DistrictId == d.Id);
                return new PublishedRateSummaryDto
                {
                    DistrictId = d.Id,
                    DistrictName = d.Name,
                   // DistrictRateId = workflowMap
                    Year = published?.Year,
                    ValidFrom = published?.ValidFrom,
                    ValidTo = published?.ValidTo,
                    CurrentWorkflowStatus = workflowMap.GetValueOrDefault(d.Id),
                    InWorkflowProcess = inWorkflow==null? false:true,
                    DistrictRateId=published?.DistrictRateId
                    
                };
            }).ToList();

            // Apply filter
            return request.Filter?.ToLower() switch
            {
                "valid" => results.Where(r => r.Year.HasValue && !r.IsExpired).ToList(),
                "expired" => results.Where(r => r.IsExpired).ToList(),
                "notpublished" => results.Where(r => !r.Year.HasValue).ToList(),
                _ => results // default = all
            };
        }
    }

    /* public class ModerationReportViewModel
     {
         public int DistrictRateId { get; set; }
         public string DistrictName { get; set; }
         public int Year { get; set; }
         public List<ModerationReportEntryViewModel> ModeratedRates { get; set; } = new List<ModerationReportEntryViewModel>();
     }

     public class ModerationReportEntryViewModel
     {
         public int CompensationRateId { get; set; }
         public string PlantName { get; set; }
         public string CategoryName { get; set; }
         public string GrowthStage { get; set; }
         public string Unit { get; set; }
         public decimal? OldRate { get; set; }
         public decimal? NewRate { get; set; }
         public bool IsChanged => OldRate != NewRate;
         public string Moderator { get; set; }
         public DateTime ModerationDate { get; set; }
         public ModerationStatus Status { get; set; }
         public bool IsDeferred { get; set; }
         public string? DeferredReason { get; set; }
     }
     public class GetModerationReportQuery : IRequest<ModerationReportViewModel>
     {
         public int DistrictRateId { get; set; }
         public string? Filter { get; set; } // "Deferred", "Pending", "Changed", "Unchanged", "Final"
     }
     public class GetModerationReportHandler : IRequestHandler<GetModerationReportQuery, ModerationReportViewModel>
     {
         private readonly IRepository<DistrictRate> _context;

         public GetModerationReportHandler(IRepository<DistrictRate> context)
         {
             _context = context;
         }

         public async Task<ModerationReportViewModel> Handle(GetModerationReportQuery request, CancellationToken cancellationToken)
         {
             var districtRate = await _context.GetAll().AsQueryable()
                 .Include(dr => dr.District)
                 .Include(dr => dr.PlantRates)
                     .ThenInclude(cr => cr.ModerationHistory)
                 .Include(dr => dr.PlantRates)
                     .ThenInclude(cr => cr.CategoryInfo)
                 .Include(dr => dr.PlantRates)
                     .ThenInclude(cr => cr.Plant)
                 .FirstOrDefaultAsync(dr => dr.Id == request.DistrictRateId, cancellationToken);

             if (districtRate == null)
                 throw new Exception("DistrictRate not found.");

             var moderatedRates = districtRate.PlantRates
                 .SelectMany(cr => cr.ModerationHistory
                     .OrderByDescending(m => m.ModerationDate) // Get latest moderation first
                     .Take(1) // Keep the most recent moderation entry
                     .Select(m => new ModerationReportEntryViewModel
                     {
                         CompensationRateId = cr.Id,
                         PlantName = string.IsNullOrEmpty(cr.Plant.CommonName)?cr.Plant.BotanicalName: cr.Plant.CommonName,
                         CategoryName = cr.CategoryInfo != null ? cr.CategoryInfo.Info[cr.CategoryInfoOption.Value] : "N/A",
                         GrowthStage = "Stage " + cr.GrowthStageId, // Assume Growth Stage ID represents a stage number
                         Unit = cr.Unit.ToString(),
                         OldRate = m.OldRate,
                         NewRate = m.NewRate,
                         Moderator = m.Moderator,
                         ModerationDate = m.ModerationDate,
                         Status = m.Status,
                         IsDeferred = m.IsDeferred,
                         DeferredReason = m.DeferredReason
                     })
                 ).ToList();

             // Apply filters if specified
             if (!string.IsNullOrEmpty(request.Filter))
             {
                 switch (request.Filter.ToLower())
                 {
                     case "deferred":
                         moderatedRates = moderatedRates.Where(m => m.IsDeferred).ToList();
                         break;
                     case "pending":
                         moderatedRates = moderatedRates.Where(m => m.Status == ModerationStatus.Pending).ToList();
                         break;
                     case "changed":
                         moderatedRates = moderatedRates.Where(m => m.IsChanged).ToList();
                         break;
                     case "unchanged":
                         moderatedRates = moderatedRates.Where(m => !m.IsChanged).ToList();
                         break;
                     case "final":
                         moderatedRates = moderatedRates.Where(m => m.Status == ModerationStatus.Approved || m.Status == ModerationStatus.Revised).ToList();
                         break;
                     default:
                         throw new Exception("Invalid filter provided.");
                 }
             }

             return new ModerationReportViewModel
             {
                 DistrictRateId = districtRate.Id,
                 DistrictName = districtRate.District.Name,
                 Year = districtRate.Year,
                 ModeratedRates = moderatedRates
             };
         }
     }
    */

}
