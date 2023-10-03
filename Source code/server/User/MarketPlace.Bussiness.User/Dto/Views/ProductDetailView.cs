using MarketPlace.DataAccess.Model;

namespace MarketPlaceUser.Bussiness.Dto.Views
{
    public class ProductDetailView : ProductView
    {
        public UserView CreatedUser { get; }

        public LocationView Location { get; }
        public ProductDetailView(Product product, string? thumbNail) : base(product, thumbNail)
        {
            CreatedUser = new UserView(product.CreatedUser);
            Location = new LocationView(product.Address, product.Latitude, product.Longitude);
        }

    }
}
