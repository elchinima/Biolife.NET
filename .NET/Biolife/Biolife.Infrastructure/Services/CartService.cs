namespace Biolife.Infrastructure.Services
{
    public class CartService
    {
        private const int MaxQuantity = 99;
        private static DateTime BakuNow => DateTime.UtcNow.AddHours(4);

        private readonly AppDbContext _db;

        public CartService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<CartItemVm>> GetItemsAsync(int userId)
        {
            var items = await _db.CartItems
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.UpdatedAt)
                .ToListAsync();

            return items.Select(ToVm).ToList();
        }

        public async Task<List<CartItemVm>> AddAsync(int userId, CartItemInputVm input)
        {
            var normalized = Normalize(input);
            var existing = await FindMatchingItemAsync(userId, normalized);

            if (existing is not null)
            {
                existing.Quantity = Math.Min(MaxQuantity, existing.Quantity + normalized.Quantity);
                existing.ProductName = normalized.ProductName;
                existing.ImageUrl = normalized.ImageUrl;
                existing.UnitPrice = normalized.UnitPrice;
                existing.UpdatedAt = BakuNow;
            }
            else
            {
                _db.CartItems.Add(new CartItem
                {
                    UserId = userId,
                    ProductId = normalized.ProductId,
                    ProductName = normalized.ProductName,
                    ImageUrl = normalized.ImageUrl,
                    UnitPrice = normalized.UnitPrice,
                    Quantity = normalized.Quantity,
                    CreatedAt = BakuNow,
                    UpdatedAt = BakuNow
                });
            }

            await _db.SaveChangesAsync();
            return await GetItemsAsync(userId);
        }

        public async Task<List<CartItemVm>?> UpdateAsync(int userId, int id, int quantity)
        {
            var item = await _db.CartItems.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            if (item is null)
                return null;

            item.Quantity = ClampQuantity(quantity);
            item.UpdatedAt = BakuNow;
            await _db.SaveChangesAsync();

            return await GetItemsAsync(userId);
        }

        public async Task<List<CartItemVm>?> RemoveAsync(int userId, int id)
        {
            var item = await _db.CartItems.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            if (item is null)
                return null;

            _db.CartItems.Remove(item);
            await _db.SaveChangesAsync();

            return await GetItemsAsync(userId);
        }

        public async Task<List<CartItemVm>> ClearAsync(int userId)
        {
            var items = await _db.CartItems.Where(c => c.UserId == userId).ToListAsync();
            if (items.Count > 0)
            {
                _db.CartItems.RemoveRange(items);
                await _db.SaveChangesAsync();
            }

            return new List<CartItemVm>();
        }

        public async Task<List<CartItemVm>> MergeAsync(int userId, IEnumerable<CartItemInputVm>? inputs)
        {
            if (inputs is null)
                return await GetItemsAsync(userId);

            foreach (var input in inputs)
                await AddAsync(userId, input);

            return await GetItemsAsync(userId);
        }

        private async Task<CartItem?> FindMatchingItemAsync(int userId, CartItemInputVm input)
        {
            if (input.ProductId.HasValue)
            {
                return await _db.CartItems.FirstOrDefaultAsync(c =>
                    c.UserId == userId &&
                    c.ProductId == input.ProductId.Value);
            }

            return await _db.CartItems.FirstOrDefaultAsync(c =>
                c.UserId == userId &&
                c.ProductId == null &&
                c.ProductName == input.ProductName &&
                c.ImageUrl == input.ImageUrl &&
                c.UnitPrice == input.UnitPrice);
        }

        private static CartItemInputVm Normalize(CartItemInputVm input)
        {
            return new CartItemInputVm
            {
                ProductId = input.ProductId,
                ProductKey = string.IsNullOrWhiteSpace(input.ProductKey) ? null : input.ProductKey.Trim(),
                ProductName = TrimTo(input.ProductName, 180, "Product"),
                ImageUrl = TrimTo(input.ImageUrl, 400, "/assets/images/products/p-01.jpg"),
                UnitPrice = input.UnitPrice < 0 ? 0 : decimal.Round(input.UnitPrice, 2),
                Quantity = ClampQuantity(input.Quantity)
            };
        }

        private static CartItemVm ToVm(CartItem item)
        {
            return new CartItemVm
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductKey = item.ProductId.HasValue
                    ? $"product:{item.ProductId.Value}"
                    : $"{item.ProductName}|{item.ImageUrl}|{item.UnitPrice:0.00}",
                ProductName = item.ProductName,
                ImageUrl = item.ImageUrl,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity
            };
        }

        private static int ClampQuantity(int quantity)
        {
            return Math.Clamp(quantity, 1, MaxQuantity);
        }

        private static string TrimTo(string? value, int maxLength, string fallback)
        {
            value = string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
            return value.Length <= maxLength ? value : value[..maxLength];
        }
    }
}
