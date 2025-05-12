using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Domain.Entities
{
    public class PlantRate:AuditableEntity
    {
       // public int Id { get; set; }


        // ✅ Single Plant or Grouped Plants
        public int? PlantId { get; set; } // Nullable to support grouped rates
        public string? PlantType { get; set; } // "Crop" or "Tree"
        public virtual Plant? Plant { get; set; }

        // ✅ Grouped plant reference
        public int? GroupedPlantId { get; set; } // NEW: Optional FK to GroupedPlant
        public virtual GroupedPlants? GroupedPlants { get; set; }

        // ✅ Named Plant Group (if applicable)
        public string? GroupName { get; set; } // If this rate applies to multiple plants
        public virtual List<PlantRateGroup> PlantRateGroups { get; set; } = new List<PlantRateGroup>(); // ✅ Uses explicit junction table

        // ✅ District & Year Tracking
        public int DistrictRateId { get; set; }
        public virtual DistrictRate DistrictRate { get; set; }

        // ✅ Growth stage where the rate applies
        public int GrowthStageId { get; set; }

        // ✅ Category (Optional)
        public int? CategoryId { get; set; }
        public int? CategoryInfoId { get; set; }
        public int? CategoryInfoOption { get; set; }
        public virtual CategoryInfo? CategoryInfo { get; set; }

        // ✅ Measurement type (Per Acre, Per Tree, etc.)
        public UnitOfMeasure Unit { get; set; }

        // ✅ Compensation Rate Details
        public decimal? Rate { get; set; }
        public string? Assumptions { get; set; }
        public string? DiscretionInfo { get; set; }
        public string? Quality { get; set; }

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
                .Where(m => m.Status == ModerationStatus.Approved|| m.Status== ModerationStatus.Revised||m.Status==ModerationStatus.NewEntry)
                .OrderByDescending(m => m.ModerationDate)
                .FirstOrDefault();
    }

        // Enum for Units of Measure
        public enum UnitOfMeasure
        {
            PerSquareMetre,
            PerTree,
            PerPlant,
            PerAcre,
            PerClump
        }
    public class PlantRateGroup
    {
        public int Id { get; set; }
        public int PlantRateId { get; set; }
        public PlantRate PlantRate { get; set; }

        public int PlantId { get; set; }
        public Plant Plant { get; set; }
    }


}

