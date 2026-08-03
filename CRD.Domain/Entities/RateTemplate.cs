namespace CRD.Domain.Entities
{
    public class RateTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// JSON-serialised RateTemplateConfig — plant items and structure items.
        /// </summary>
        public string ConfigJson { get; set; } = "{}";
    }
}
