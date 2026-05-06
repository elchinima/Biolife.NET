namespace Biolife.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public decimal Price { get; set; }
        public decimal CostPrice { get; set; }
        public decimal DiscountPercent { get; set; }

        public string ImageUrl { get; set; } = null!;
        public string? HoverImageUrl { get; set; }

        public bool IsFeatured { get; set; }
        public bool IsNew { get; set; }
        public int ViewCount { get; set; }

        public DateTime? SaleEndDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int GenreId { get; set; }
        public Genre Genre { get; set; } = null!;

        public int TagId { get; set; }
        public Tag Tag { get; set; } = null!;

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}

