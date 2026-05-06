namespace Biolife.Domain.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Bio { get; set; }
        public string? ImageUrl { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}

