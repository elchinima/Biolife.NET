namespace Biolife.Domain.Entities
{
    public class CartItem
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int? ProductId { get; set; }
        public Product? Product { get; set; }

        [MaxLength(180)]
        public string ProductName { get; set; } = null!;

        [MaxLength(400)]
        public string ImageUrl { get; set; } = null!;

        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(4);
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow.AddHours(4);
    }
}
