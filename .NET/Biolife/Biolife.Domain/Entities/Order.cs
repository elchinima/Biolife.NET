namespace Biolife.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [MaxLength(120)]
        public string FullName { get; set; } = null!;
        [MaxLength(120)]
        public string Email { get; set; } = null!;
        [MaxLength(30)]
        public string PhoneNumber { get; set; } = null!;
        [MaxLength(180)]
        public string AddressLine1 { get; set; } = null!;
        [MaxLength(180)]
        public string? AddressLine2 { get; set; }
        [MaxLength(80)]
        public string Country { get; set; } = null!;
        [MaxLength(80)]
        public string City { get; set; } = null!;
        public decimal Subtotal { get; set; }
        [MaxLength(30)]
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(4);
        public User User { get; set; } = null!;
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
