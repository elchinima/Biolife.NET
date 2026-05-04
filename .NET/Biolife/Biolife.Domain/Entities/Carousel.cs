namespace Biolife.Domain.Entities
{
    public class Carousel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Decription { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public string ButtonText { get; set; } = null!;
        public string ButtonLink { get; set; } = null!;
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
}
