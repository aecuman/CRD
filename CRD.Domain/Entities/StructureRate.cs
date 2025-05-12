using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Domain.Entities
{
    public class StructureRate
    {
        public int Id { get; set; }
        public int DistrictRateId { get; set; }
        public virtual DistrictRate DistrictRate { get; set; }
        public int StructureId { get; set; }
        public virtual Structure Structure { get; set; }
        public StructuresUnitOfMeasure Unit { get; set; }

        // ✅ Compensation Rate Details
        public decimal? Rate { get; set; }
        public string? Assumptions { get; set; }
        public string? DiscretionInfo { get; set; }
        // ✅ Moderation status
        public ModerationStatus Status { get; set; } = ModerationStatus.Pending;

        // ✅ Moderation history tracking
        public virtual List<CompensationRateModeration> ModerationHistory { get; set; } = new List<CompensationRateModeration>();

        // Tracks whether a review has been deferred
        public bool IsReviewDeferred { get; set; } = false;

        // Notes for deferred review
        public string? DeferredReviewReason { get; set; }

        // Time when the review was deferred (if applicable)
        public DateTime? DeferredReviewDate { get; set; }
        // 🧠 Returns the most recently approved moderation
        [NotMapped]
        public CompensationRateModeration? LatestApprovedModeration =>
            ModerationHistory?
                .Where(m => m.Status == ModerationStatus.Approved)
                .OrderByDescending(m => m.ModerationDate)
                .FirstOrDefault();

    }

    public enum StructuresUnitOfMeasure
    {
        PerSquareMetre,
        PerMetre,
        PerUnit,
        PerFoot,
        PerCubicMetre
    }
}
