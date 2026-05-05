namespace Biolife.Application.ViewModels
{
    public class CartItemVm
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public string ProductKey { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
    }
}
