using MarketPlace.DataAccess.Model;

namespace MarketPlaceAdmin.Bussiness.Dto.Views
{
    public class ProductDetailView : ProductView
    {
        public UserView CreatedUser { get; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public ProductDetailView(Product product) : base(product, product.Photos?.FirstOrDefault()?.Photo)
        {
            CreatedUser = new UserView(product.CreatedUser);
            Longitude = product.Longitude;
            Latitude = product.Latitude;
        }

        public ProductDetailView(Product product, string? thumbnail) : base(product, thumbnail)
        {
            CreatedUser = new UserView(product.CreatedUser);
            Longitude = product.Longitude;
            Latitude = product.Latitude;
        }

    }
}
