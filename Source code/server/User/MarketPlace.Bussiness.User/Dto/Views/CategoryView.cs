using MarketPlace.DataAccess.Model;

namespace MarketPlaceUser.Bussiness.Dto.Views
{
    public class CategoryView
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public CategoryView(Category category)
        {
            CategoryId = category.CategoryId;
            CategoryName = category.CategoryName;
        }
    }
}
