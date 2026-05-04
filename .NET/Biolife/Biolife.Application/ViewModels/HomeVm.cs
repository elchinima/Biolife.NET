namespace Biolife.Application.ViewModels
{
    public class HomeVm
    {
        public List<Carousel> Carousels { get; set; } = new();
        public List<Product> FeaturedProducts { get; set; } = new();
        public List<Product> NewArrivals { get; set; } = new();
        public List<Product> MostViewed { get; set; } = new();
        public List<Product> DealOfDay { get; set; } = new();
        public List<Product> BestSellers { get; set; } = new();
        public List<Product> ChildrenProducts { get; set; } = new();
        public List<Product> ArtsProducts { get; set; } = new();
    }
}
