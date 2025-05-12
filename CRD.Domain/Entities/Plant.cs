using CRD.Domain.Process;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Domain.Entities
{
    public class Plant
    {
        public Plant() {
        Images = new List<CRDFile>();
        }
        public int Id { get; set; }
        public string? CommonName { get; set; }
        public string? PlantType { get; set; }
        public string BotanicalName { get; set; }
        public  List<Translation> Translations { get; set; }=new List<Translation>();
        public  List<int> GrowthStages { get; set; }=new List<int>();
        public List<int> Categories { get; set; }=new List<int>();
        public List<CategoryInfo> CategoryInform {  get; set; } =new List<CategoryInfo>();
        public string? Info { get; set; }
        public List<CRDFile> Images { get;} = new List<CRDFile>();

        // Navigation to compensation rates (one plant can have multiple rates)
        public virtual ICollection<PlantRate> PlantRates { get; set; }
        public virtual ICollection<GroupedPlantItem> GroupedPlantItems { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
    public class GroupedPlants
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<GroupedPlantItem> GroupedPlantItems { get; set; } = new List<GroupedPlantItem>();
        public List<int> GrowthStages { get; set; } = new List<int>();
    }
    public class GroupedPlantItem
    {
        public int Id { get; set; } // recommended primary key
        public int GroupedPlantId { get; set; }
        public GroupedPlants GroupedPlant { get; set; }
        public int? CropId { get; set; }
        public Crop Crop { get; set; }

        public int? TreeId { get; set; }
        public Tree Tree { get; set; }
    }
}
