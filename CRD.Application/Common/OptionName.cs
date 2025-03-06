using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Common
{
    public struct OptionName
    {

        public const string StructureCategoryOptionName = "structure_category";
        public const string StructureTypeOptionName = "structure_type";
        public const string StructureDescriptionOptionName = "structure_description_option";
        public const string StructureDescriptionNameOptionName = "structure_description_name";
        public const string PlantCategoryOptionName = "plant_category";
        public const string LanguangeOptionName = "plant_languange";
        public const string GrowthStageOptionName = "plant_growthstage";

        public static readonly string[] Value = new[] { OptionName.GrowthStageOptionName, OptionName.LanguangeOptionName, OptionName.StructureCategoryOptionName, OptionName.StructureDescriptionOptionName, OptionName.StructureDescriptionNameOptionName, OptionName.PlantCategoryOptionName };
        // public static readonly KeyValuePair<string, string>[] Value = new[] { new KeyValuePair<string, string>(nameof(OptionName.StructureCategoryOptionName), OptionName.) };
    }
}
