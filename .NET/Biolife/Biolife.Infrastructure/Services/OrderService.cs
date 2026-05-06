namespace Biolife.Infrastructure.Services
{
    public class OrderService
    {
        private readonly AppDbContext _db;

        public OrderService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Order?> CreateFromCartAsync(int userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u =>
                u.Id == userId &&
                u.IsActive &&
                u.EmailConfirmed);

            if (user is null)
                return null;

            var cartItems = await _db.CartItems
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.Id)
                .ToListAsync();

            if (cartItems.Count == 0)
                return null;

            var order = new Order
            {
                UserId = user.Id,
                FullName = user.FullName!,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber!,
                AddressLine1 = user.AddressLine1!,
                AddressLine2 = user.AddressLine2,
                Country = user.Country!,
                City = user.City!,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow.AddHours(4),
                Subtotal = cartItems.Sum(item => item.UnitPrice * item.Quantity),
                Items = cartItems.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ImageUrl = item.ImageUrl,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    LineTotal = item.UnitPrice * item.Quantity
                }).ToList()
            };

            _db.Orders.Add(order);
            _db.CartItems.RemoveRange(cartItems);
            await _db.SaveChangesAsync();

            return order;
        }
    }
}
