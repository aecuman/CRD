namespace CRD.Domain.Entities
{
    // Class to track moderation changes
    public class CompensationRateModeration
    {
        public int Id { get; set; }

        // Reference to the moderated rate (either PlantRate or StructureRate, but not both)
        public int? PlantRateId { get; set; }
        public virtual PlantRate? PlantRate { get; set; }

        public int? StructureRateId { get; set; }
        public virtual StructureRate? StructureRate { get; set; }

        // Optional: Old rate may not exist if this is a fresh discretionary entry
        public decimal? OldRate { get; set; }

        // Optional: New rate might not exist; clarification could instead be via DiscretionInfo
        public decimal? NewRate { get; set; }

        // Moderation reasoning
        public string ModerationNotes { get; set; } = string.Empty;

        // Moderator identity
        public string Moderator { get; set; } = string.Empty;

        // Timestamp of moderation
        public DateTime ModerationDate { get; set; } = DateTime.UtcNow;

        // Status of moderation
        public ModerationStatus Status { get; set; } = ModerationStatus.Pending;

        // Deferment logic
        public bool IsDeferred { get; set; } = false;
        public string? DeferredReason { get; set; }

        // Optional discretionary justification (used when numeric rates are absent)
        public string? NewDiscretionInfo { get; set; } // Proposed new discretionary info
        public string? DiscretionInfo { get; set; }     // Snapshot of old discretionary info

        // Helpers
        public bool IsLinkedToPlant => PlantRateId.HasValue;
        public bool IsLinkedToStructure => StructureRateId.HasValue;
        public bool IsValid => PlantRateId.HasValue ^ StructureRateId.HasValue;
        public bool HasDiscretionaryOnly => !OldRate.HasValue && !NewRate.HasValue && !string.IsNullOrWhiteSpace(DiscretionInfo);

    }



}
// Enum to track moderation status
public enum ModerationStatus
{
    Pending,      // Awaiting review
    Approved,     // Accepted and finalised
    Rejected,     // Declined – not acceptable
    Revised,      // Modified after review suggestions
    Deferred,     // Deferred – for later moderation
    Deleted,      // Removed (e.g., mistake or invalid)
    NewEntry      // First-time entry – no previous rate exists
}
public enum RateType
{
    Plant,
    Structure
}


