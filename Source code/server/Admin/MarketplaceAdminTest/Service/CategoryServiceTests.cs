using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Dto.Views;
using MarketPlaceAdmin.Bussiness.Enums;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Services;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace MarketplaceAdminTest.Service
{


    public class CategoryServiceTests
    {

        private readonly CategoryService _categoryService;
        private readonly IUnitOfWork _uow;

        public CategoryServiceTests()
        {
            _uow = Substitute.For<IUnitOfWork>();
            _categoryService = new CategoryService(_uow);
        }

        [Fact]
        public async Task AddCategory_WhenCategoryDoesNotExist_ShouldReturnSuccess()
        {
            // Arrange
            var categoryForm = new CategoryForm { CategoryName = "TestCategory" };

            _uow.CategoryRepository.FindByCategoryNameAsync(Arg.Any<string>()).ReturnsNull();
            _uow.CategoryRepository.Add(Arg.Any<Category>()).Returns(new Category() { CategoryName = categoryForm.CategoryName });

            // Act
            var result = await _categoryService.AddCategory(categoryForm);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Category Added", result.Message);
            Assert.NotNull(result.Data);
            Assert.IsType<CategoryDetailView>(result.Data);
            Assert.Equal(categoryForm.CategoryName.Trim(), ((CategoryDetailView)result.Data).CategoryName);
            await _uow.CategoryRepository.Received().Add(Arg.Any<Category>());
            await _uow.Received().SaveAsync();
        }

        [Fact]
        public async Task AddCategory_WhenCategoryAlreadyExists_ShouldReturnAlreadyExists()
        {
            // Arrange
            var categoryForm = new CategoryForm { CategoryName = "TestCategory" };

            _uow.CategoryRepository.FindByCategoryNameAsync(Arg.Any<string>()).Returns(new Category());

            // Act
            var result = await _categoryService.AddCategory(categoryForm);

            // Assert
            Assert.Equal(ServiceStatus.AlreadyExists, result.ServiceStatus);
            Assert.Equal("Category Already Exists", result.Message);
            Assert.Null(result.Data);
            await _uow.CategoryRepository.DidNotReceive().Add(Arg.Any<Category>());
            await _uow.DidNotReceive().SaveAsync();
        }

        [Fact]
        public async Task EditCategory_ShouldReturnNotFound_WhenCategoryIdDoesNotExist()
        {
            // Arrange
            int categoryId = 1;
            CategoryForm form = new CategoryForm { CategoryName = "Category 1" };
            _uow.CategoryRepository
                .FindByIdAndStatusAsync(categoryId, Category.CategoryStatus.ACTIVE)
                .Returns(Task.FromResult<Category?>(null));

            // Act
            var result = await _categoryService.EditCategory(form, categoryId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Category Not Found", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task EditCategory_ShouldReturnBadRequest_WhenCategoryIsInactive()
        {
            // Arrange
            int categoryId = 1;
            CategoryForm form = new CategoryForm { CategoryName = "Category 1" };
            _uow.CategoryRepository
                .FindByIdAndStatusAsync(categoryId, Category.CategoryStatus.ACTIVE)
                .Returns(new Category { CategoryId = categoryId, Status = Category.CategoryStatus.INACTIVE });

            // Act
            var result = await _categoryService.EditCategory(form, categoryId);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Inactive Category", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task EditCategory_ShouldReturnAlreadyExists_WhenCategoryNameAlreadyExists()
        {
            // Arrange
            int categoryId = 1;
            string categoryName = "Category 1";
            CategoryForm form = new CategoryForm { CategoryName = categoryName };
            _uow.CategoryRepository.FindByCategoryNameAsync(categoryName)
                .Returns(new Category { CategoryId = 2, CategoryName = categoryName });
            _uow.CategoryRepository.FindByIdAndStatusAsync(categoryId, Category.CategoryStatus.ACTIVE)
                .Returns(new Category { CategoryId = categoryId, CategoryName = "Category 2", Status = Category.CategoryStatus.ACTIVE });

            // Act
            var result = await _categoryService.EditCategory(form, categoryId);

            // Assert
            Assert.Equal(ServiceStatus.AlreadyExists, result.ServiceStatus);
            Assert.Equal("Category Already Exists", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task EditCategory_ShouldReturnSuccess_WhenCategoryIsUpdatedSuccessfully()
        {
            // Arrange
            int categoryId = 1;
            string categoryName = "Category 1";
            CategoryForm form = new CategoryForm { CategoryName = categoryName };
            Category category = new Category { CategoryId = categoryId, CategoryName = "Category 2", Status = Category.CategoryStatus.ACTIVE };
            _uow.CategoryRepository.FindByCategoryNameAsync(categoryName)
                .Returns(Task.FromResult<Category?>(null));
            _uow.CategoryRepository.FindByIdAndStatusAsync(categoryId, Category.CategoryStatus.ACTIVE)
                .Returns(category);

            // Act
            var result = await _categoryService.EditCategory(form, categoryId);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Category Name Updated", result.Message);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task GetCategoryList_ReturnsSuccessResult_WhenCategoriesExist()
        {
            // Arrange
            List<Category> categories = new List<Category>()
        {
            new Category() { CategoryId = 1, CategoryName = "Category 1", Status = Category.CategoryStatus.ACTIVE },
            new Category() { CategoryId = 2, CategoryName = "Category 2", Status = Category.CategoryStatus.ACTIVE },
        };
            _uow.CategoryRepository.FindAllAsync().Returns(categories);

            // Act
            ServiceResult result = await _categoryService.GetCategoryList();

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Category List", result.Message);
            Assert.NotNull(result.Data);
            Assert.Equal(2, ((List<CategoryDetailView>)result.Data).Count);
        }

        [Fact]
        public async Task GetCategoryList_ReturnsEmptyList_WhenNoCategoriesExist()
        {
            // Arrange
            _uow.CategoryRepository.FindAllAsync().Returns(new List<Category>());

            // Act
            ServiceResult result = await _categoryService.GetCategoryList();

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Category List", result.Message);
            Assert.NotNull(result.Data);
            Assert.Empty((List<CategoryDetailView>)result.Data);
        }

        [Fact]
        public async Task ChangeCategoryStatus_WhenCategoryNotFound_ReturnsNotFoundServiceResult()
        {
            // Arrange
            int categoryId = 1;
            byte status = (byte)Category.CategoryStatus.ACTIVE;
            Category? category = null;
            _uow.CategoryRepository.FindById(categoryId).Returns(category);

            // Act
            ServiceResult result = await _categoryService.ChangeCategoryStatus(categoryId, status);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Category Not Found", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task ChangeCategoryStatus_WhenInvalidStatus_ReturnsBadRequestServiceResult()
        {
            // Arrange
            int categoryId = 1;
            byte status = 3;
            Category category = new Category();
            _uow.CategoryRepository.FindById(categoryId).Returns(category);

            // Act
            ServiceResult result = await _categoryService.ChangeCategoryStatus(categoryId, status);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Status", result.Message);
            Assert.Null(result.Data);
        }



        [Fact]
        public async Task ChangeCategoryStatus_WhenStatusChanged_ReturnsSuccessServiceResult()
        {
            // Arrange
            int categoryId = 1;
            byte status = (byte)Category.CategoryStatus.INACTIVE;
            Category category = new Category();
            _uow.CategoryRepository.FindById(categoryId).Returns(category);

            // Act
            ServiceResult result = await _categoryService.ChangeCategoryStatus(categoryId, status);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Status Changed", result.Message);
            Assert.NotNull(result.Data);
            Assert.IsType<CategoryDetailView>(result.Data);
        }

        [Fact]
        public async Task EditCategory_WithValidData_ReturnsSuccessResultWithUpdatedCategoryDetails()
        {
            // Arrange
            var form = new CategoryForm { CategoryName = "New Category Name" };
            var categoryId = 1;
            var originalCategory = new Category
            {
                CategoryId = categoryId,
                CategoryName = "Original Category Name",
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                Status = Category.CategoryStatus.ACTIVE
            };

            _uow.CategoryRepository.FindByCategoryNameAsync(form.CategoryName.Trim()).Returns((Category?)null);
            _uow.CategoryRepository.FindByIdAndStatusAsync(categoryId, Category.CategoryStatus.ACTIVE).Returns(originalCategory);
            await _uow.CategoryRepository.FindAllByStatusAsync(Category.CategoryStatus.ACTIVE);

            // Act
            var result = await _categoryService.EditCategory(form, categoryId);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Category Name Updated", result.Message);

            var updatedCategory = Assert.IsType<CategoryDetailView>(result.Data);
            Assert.Equal(originalCategory.CategoryId, updatedCategory.CategoryId);
            Assert.Equal(form.CategoryName.Trim(), updatedCategory.CategoryName);
            Assert.Equal((byte)Category.CategoryStatus.ACTIVE, updatedCategory.Status);
            Assert.Equal((byte)originalCategory.Status, updatedCategory.Status);

            _uow.CategoryRepository.Received(1).Update(Arg.Is<Category>(c => c.CategoryId == categoryId && c.CategoryName == form.CategoryName.Trim()));
            await _uow.Received(1).SaveAsync();

        }
        [Fact]
        public async Task EditCategory_WithDuplicateCategoryName_ReturnsAlreadyExistsResult()
        {
            // Arrange
            var form = new CategoryForm { CategoryName = "Existing Category" };
            var categoryId = 1;
            var existingCategory = new Category
            {
                CategoryId = categoryId + 1,
                CategoryName = form.CategoryName,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                Status = Category.CategoryStatus.ACTIVE
            };

            _uow.CategoryRepository.FindByCategoryNameAsync(form.CategoryName.Trim()).Returns(existingCategory);

            // Act
            var result = await _categoryService.EditCategory(form, categoryId);

            // Assert
            Assert.Equal(ServiceStatus.AlreadyExists, result.ServiceStatus);
            Assert.Equal("Category Already Exists", result.Message);
            Assert.Null(result.Data);

            _uow.CategoryRepository.DidNotReceive().Update(Arg.Any<Category>());
            await _uow.DidNotReceive().SaveAsync();

        }
        [Fact]
        public async Task EditCategory_WithInactiveCategory_ReturnsBadRequestResult()
        {
            // Arrange
            var form = new CategoryForm { CategoryName = "New Category Name" };
            var categoryId = 1;
            var inactiveCategory = new Category
            {
                CategoryId = categoryId,
                CategoryName = "Inactive Category",
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                Status = Category.CategoryStatus.INACTIVE
            };

            _uow.CategoryRepository.FindByCategoryNameAsync(form.CategoryName.Trim()).Returns((Category?)null);
            _uow.CategoryRepository.FindByIdAndStatusAsync(categoryId, Category.CategoryStatus.ACTIVE).Returns(inactiveCategory);

            // Act
            var result = await _categoryService.EditCategory(form, categoryId);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Inactive Category", result.Message);
            Assert.Null(result.Data);

            _uow.CategoryRepository.DidNotReceive().Update(Arg.Any<Category>());
            await _uow.DidNotReceive().SaveAsync();

        }
    }
}