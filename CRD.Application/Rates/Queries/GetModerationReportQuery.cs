using CRD.Application.Common;
using CRD.Application.Common.Dtos;
using CRD.Application.StructureRates.Queries;
using CRD.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CRD.Application.Rates.Queries
{
    public class ModerationReportViewModel
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public int DistrictRateId { get; set; }
        public string DistrictName { get; set; }
        public int DistrictId { get; set; }
        public int Year { get; set; }

        public List<ModeratedPlantRateViewModel> PlantRates { get; set; } = new();
        public List<ModeratedStructureRateViewModel> StructureRates { get; set; } = new();
        public DateTime PublishedOn { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
    }
    public class GetModerationReportQuery : IRequest<ModerationReportViewModel>
    {
        public int DistrictRateId { get; set; }
        public string? Filter { get; set; } // "Deferred", "Pending", "Changed", "Unchanged", "Final"
    }

    public partial class GetModerationReportHandler : IRequestHandler<GetModerationReportQuery, ModerationReportViewModel>
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
                .Include(dr => dr.PlantRates).ThenInclude(pr => pr.ModerationHistory)
                .Include(dr => dr.PlantRates).ThenInclude(pr => pr.Plant)
                .Include(dr => dr.PlantRates).ThenInclude(pr => pr.CategoryInfo)
                .Include(dr => dr.StructureRates).ThenInclude(sr => sr.ModerationHistory)
                .Include(dr => dr.StructureRates).ThenInclude(sr => sr.Structure).ThenInclude(s => s.Category).ThenInclude(c=>c.Attributes).ThenInclude(a=>a.Options)
                .Include(dr => dr.StructureRates).ThenInclude(sr => sr.Structure.StructureType)
                .Include(dr => dr.StructureRates).ThenInclude(sr => sr.Structure.AttributeSelections).ThenInclude(a => a.OptionSelections)
                .FirstOrDefaultAsync(dr => dr.Id == request.DistrictRateId, cancellationToken);

            if (districtRate == null)
                throw new Exception("DistrictRate not found.");

            var filteredPlants = districtRate.PlantRates
                .Select(pr =>
                {
                    var first = pr.ModerationHistory.OrderBy(m => m.ModerationDate).FirstOrDefault();
                    var latest = pr.ModerationHistory.OrderByDescending(m => m.ModerationDate).FirstOrDefault();

                    return latest == null ? null : new ModeratedPlantRateViewModel
                    {
                        Id = pr.Id,
                        GroupedPlantId = pr.GroupedPlantId,
                        GroupName = pr.GroupName,
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
                        Status = pr.Status.ToString(),
                        IsModerated = true,
                        OriginalRate = first?.OldRate ?? pr.Rate,
                        LatestModeration = new ModerationDetailViewModel
                        {
                            OldRate = latest.OldRate,
                            NewRate = latest.NewRate,
                            ModerationNotes = latest.ModerationNotes,
                            Moderator = latest.Moderator,
                            ModerationDate = latest.ModerationDate,
                            DiscretionaryInfo = latest.DiscretionInfo,
                            DeferredReason = latest.IsDeferred ? latest.DeferredReason : null,
                            Status = latest.Status.ToString()
                        }
                    };
                })
                .Where(p => p != null)
                .ToList();

            var filteredStructures = districtRate.StructureRates
                .Select(sr =>
                {
                    var first = sr.ModerationHistory.OrderBy(m => m.ModerationDate).FirstOrDefault();
                    var latest = sr.ModerationHistory.OrderByDescending(m => m.ModerationDate).FirstOrDefault();

                    return latest == null ? null : new ModeratedStructureRateViewModel
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
                        DiscretionInfo = sr.DiscretionInfo,
                        Status = sr.Status.ToString(),
                        IsModerated = true,
                        OriginalRate = first?.OldRate ?? sr.Rate,
                        LatestModeration = new ModerationDetailViewModel
                        {
                            OldRate = latest.OldRate,
                            NewRate = latest.NewRate,
                            ModerationNotes = latest.ModerationNotes,
                            Moderator = latest.Moderator,
                            ModerationDate = latest.ModerationDate,
                            DiscretionaryInfo = latest.DiscretionInfo,
                            DeferredReason = latest.IsDeferred ? latest.DeferredReason : null,
                            Status = latest.Status.ToString()
                        }
                    };
                })
                .Where(s => s != null)
                .ToList();

            // 🧠 Apply Filters to Both Lists
            if (!string.IsNullOrEmpty(request.Filter))
            {
                Func<ModerationDetailViewModel?, ModerationStatus?, bool> matches = (m, status) =>
                {
                    if (m == null) return false;
                    return request.Filter.ToLower() switch
                    {
                        "deferred" => !string.IsNullOrEmpty(m.DeferredReason),
                        "pending" => m.Status == ModerationStatus.Pending.ToString(),
                        "changed" => m.OldRate != m.NewRate,
                        "unchanged" => m.OldRate == m.NewRate,
                        "final" => m.Status == ModerationStatus.Approved.ToString() || m.Status == ModerationStatus.Revised.ToString(),
                        _ => true
                    };
                };

                filteredPlants = filteredPlants
                    .Where(p => matches(p.LatestModeration, p.Status.ParseEnumSafe<ModerationStatus>()))
                    .ToList();

                filteredStructures = filteredStructures
                    .Where(s => matches(s.LatestModeration, s.Status.ParseEnumSafe<ModerationStatus>()))
                    .ToList();
            }

            return new ModerationReportViewModel
            {
                DistrictRateId = districtRate.Id,
                DistrictName = districtRate.District.Name,
                DistrictId = districtRate.District.Id,
                Year = districtRate.Year,
                PlantRates = filteredPlants,
                StructureRates = filteredStructures
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
