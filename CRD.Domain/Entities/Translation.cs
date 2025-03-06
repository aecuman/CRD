namespace CRD.Domain.Entities
{
    public class Translation
    {
        public int Id { get; set; }
        public int LanguageId { get; set; }
        public int? CropId { get; set; }
        public int? TreeId { get; set; } 
        public string Translated { get; set; }
    }
    public class Languange
    {
        public int Id { get; private set; }
        public string Name { get; set; }
    }
}