namespace Biolife.Application.ViewModels
{
    public class AdminVm
    {
        public int TotalProducts { get; set; }
        public int TotalTags { get; set; }
        public int TotalGenres { get; set; }
        public int TotalCarousels { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }

        public List<Product> Products { get; set; } = new();
        public List<Tag> Tags { get; set; } = new();
        public List<Genre> Genres { get; set; } = new();
        public List<Carousel> Carousels { get; set; } = new();
        public List<User> Users { get; set; } = new();
        public List<Role> Roles { get; set; } = new();
        public List<Note> Notes { get; set; } = new();
        public List<Order> Orders { get; set; } = new();
        public int TotalUsers { get; set; }
        public Role? CurrentUserRole { get; set; }
    }
}

