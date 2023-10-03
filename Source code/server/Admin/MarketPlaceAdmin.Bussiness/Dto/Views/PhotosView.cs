using MarketPlace.DataAccess.Model;

namespace MarketPlaceAdmin.Bussiness.Dto.Views
{
    public class PhotosView
    {
        public int PhotosId { get; set; }

        public int ProductId { get; set; }

        public string Photo { get; set; }

        public PhotosView(Photos photos)
        {
            PhotosId = photos.PhotosId;
            ProductId = photos.ProductId;
            Photo = photos.Photo;
        }
    }
}
