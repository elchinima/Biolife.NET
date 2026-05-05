namespace Biolife.Application.ViewModels
{
    public class CartItemInputVm
    {
        public int? Id { get; set; }
        public int? ProductId { get; set; }
        public string? ProductKey { get; set; }
        public string ProductName { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
