using MarketPlace.DataAccess.Model;

namespace MarketPlaceUser.Bussiness.Dto.Views
{
    public class ProductCartWishListView : ProductView
    {
        public byte CreatedUserStatus { get; }
        public ProductCartWishListView(Product product, string? thumbnail) : base(product, thumbnail)
        {
            CreatedUserStatus = (byte)product.CreatedUser.Status;
        }
    }
}
