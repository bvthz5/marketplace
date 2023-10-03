using MarketPlace.DataAccess.Model;

namespace MarketPlaceAdmin.Bussiness.Dto.Views
{
    public class CategoryDetailView
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public byte Status { get; }

        public CategoryDetailView(Category category)
        {
            CategoryId = category.CategoryId;
            Status = (byte)category.Status;
            CategoryName = category.CategoryName;
        }
    }
}
