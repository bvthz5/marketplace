using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Bussiness.Dto.Views;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;

namespace MarketPlaceUser.Bussiness.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _uow;

        public CategoryService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        /// <summary>
        /// Retrieves a list of all active categories.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation. The result is a <see cref="ServiceResult"/> object containing the list of active categories.</returns>
        public async Task<ServiceResult> GetActiveCategoryList()
        {
            ServiceResult result = new()
            {
                Data = (await _uow.CategoryRepository.FindAllByStatusAsync(Category.CategoryStatus.ACTIVE))
                                        .ConvertAll(category => new CategoryView(category))
            };

            return result;
        }


    }
}
