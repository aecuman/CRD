namespace CRD.Application.Templates
{
    /// <summary>Full configuration stored in RateTemplate.ConfigJson</summary>
    public class RateTemplateConfig
    {
        public List<PlantTemplateItem> PlantItems { get; set; } = new();
        public List<StructureTemplateItem> StructureItems { get; set; } = new();
    }

    public class PlantTemplateItem
    {
        public int? PlantId { get; set; }
        public string? PlantType { get; set; }
        public int? GroupedPlantId { get; set; }
        public string? GroupName { get; set; }
        public List<int>? GroupedPlantIds { get; set; }
        public List<int> GrowthStageIds { get; set; } = new();
        public List<int> Units { get; set; } = new();
        public List<string> Qualities { get; set; } = new() { "Good" };
        public List<TemplateCategoryInfoSelection>? CategoryInfos { get; set; }
    }

    public class TemplateCategoryInfoSelection
    {
        public int CategoryId { get; set; }
        public int CategoryInfoId { get; set; }
        public int CategoryInfoOption { get; set; }
    }

    public class StructureTemplateItem
    {
        public int StructureId { get; set; }
        public int Unit { get; set; }
    }

    public class RateTemplateViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public RateTemplateConfig Config { get; set; } = new();
        public int PlantCount { get; set; }
        public int StructureCount { get; set; }
    }
}
