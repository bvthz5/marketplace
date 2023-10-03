using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Bussiness.Dto.Views;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Services;
using NSubstitute;

namespace MarketPlaceUserTest.Servicetest
{
    public class CategoryServiceTests
    {
        [Fact]
        public async Task GetActiveCategoryList_ReturnsListOfActiveCategories()
        {
            // Arrange
            var uow = Substitute.For<IUnitOfWork>();
            var categoryRepository = Substitute.For<ICategoryRepository>();
            uow.CategoryRepository.Returns(categoryRepository);
            var categories = new List<Category>
            {
                new Category
                {
                    CategoryId = 1,
                    CategoryName = "Category 1",
                    Status = Category.CategoryStatus.ACTIVE
                }
            };
            categoryRepository.FindAllByStatusAsync(Category.CategoryStatus.ACTIVE).Returns(categories);

            var categoryService = new CategoryService(uow);

            // Act
            var result = await categoryService.GetActiveCategoryList();



            // Assert
            Assert.NotNull(result);
            Assert.IsType<ServiceResult>(result);
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.NotNull(result.Data);
            Assert.IsType<List<CategoryView>>(result.Data);
            Assert.True(result.Status); 
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
        }
    }
}
