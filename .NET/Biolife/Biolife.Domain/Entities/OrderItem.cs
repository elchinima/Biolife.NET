namespace Biolife.Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int? ProductId { get; set; }
        [MaxLength(180)]
        public string ProductName { get; set; } = null!;
        [MaxLength(400)]
        public string ImageUrl { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
        public Order Order { get; set; } = null!;
        public Product? Product { get; set; }
    }
}
