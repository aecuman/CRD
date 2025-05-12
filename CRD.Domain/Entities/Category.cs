namespace CRD.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class CategoryInfo
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public int? TreeId { get; set; }
        public int? CropId { get; set; }
        public string[] Info { get; set; }


    }
}