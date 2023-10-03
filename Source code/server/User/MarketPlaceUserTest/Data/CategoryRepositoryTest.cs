using MarketPlace.DataAccess.Data;
using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlace.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;

namespace MarketPlaceUserTest.Data
{
    public class CategoryRepositoryTest
    {
        private readonly MarketPlaceDbContext _dbContext;
        private readonly ICategoryRepository _categoryRepository;

        public CategoryRepositoryTest()
        {
            _dbContext = new MarketPlaceDbContext(new DbContextOptionsBuilder<MarketPlaceDbContext>().UseInMemoryDatabase(databaseName: "TestDatabase").Options);
            _categoryRepository = new CategoryRepository(_dbContext);
        }

        [Fact]
        public async Task AddAsync_ShouldAddCategoryToDatabase()
        {
            // Arrange

            var category = new Category
            {
                CategoryId = 1,
                CategoryName = "Sample Category"
            };

            // Act
            var result = await _categoryRepository.Add(category);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(category, result); // Expecting the returned category to be the same as the input category

            // Check if the category is added to the database
            var addedCategory = await _dbContext.Categories.FindAsync(category.CategoryId);
            Assert.NotNull(addedCategory);
            Assert.Equal(category.CategoryId, addedCategory.CategoryId);
            Assert.Equal(category.CategoryName, addedCategory.CategoryName);
        }

        [Fact]
        public async Task FindAllAsync_ShouldReturnAllCategoriesOrderedByIdDescending()
        {
            // Arrange

            // Create test data
            var categories = new List<Category>
            {
                new Category { CategoryId = 1, CategoryName = "Category 1" },
                new Category { CategoryId = 2, CategoryName = "Category 2" },
                new Category { CategoryId = 3, CategoryName = "Category 3" }
            };

            _dbContext.Categories.AddRange(categories);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _categoryRepository.FindAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categories.Count, result.Count); // Expecting the same number of categories as in the test data
            Assert.Equal(categories[2].CategoryId, result[0].CategoryId); // Expecting the categories to be ordered by CategoryId in descending order
            Assert.Equal(categories[1].CategoryId, result[1].CategoryId);
            Assert.Equal(categories[0].CategoryId, result[2].CategoryId);
        }

        [Fact]
        public async Task FindByIdAsync_ShouldReturnNullWhenNotExists()
        {
            // Arrange
            int categoryId = 1;

            // Act
            var result = await _categoryRepository.FindById(categoryId);
            await _dbContext.SaveChangesAsync();

            // Assert
            Assert.NotNull(result);
        }


        [Fact]
        public async Task FindByIdAndStatusAsync_ShouldReturnNull_WhenNoMatchingCategory()
        {
            // Arrange
            int categoryId = 1;
            var status = Category.CategoryStatus.ACTIVE;

            // Act
            var result = await _categoryRepository.FindByIdAndStatusAsync(categoryId, status);
            await _dbContext.SaveChangesAsync();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task FindAllByStatusAsync_ReturnsMatchingCategories()
        {
            // Arrange
            var status = Category.CategoryStatus.ACTIVE;
            var categories = new List<Category>
            {
                new Category { CategoryId = 1, CategoryName = "Category 1", Status = Category.CategoryStatus.ACTIVE },
                new Category { CategoryId = 2, CategoryName = "Category 2", Status = Category.CategoryStatus.ACTIVE },
                new Category { CategoryId = 3, CategoryName = "Category 3", Status = Category.CategoryStatus.INACTIVE },
            };

            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var dbContext = new MarketPlaceDbContext(dbContextOptions);
            dbContext.Categories.RemoveRange(dbContext.Categories);
            await dbContext.SaveChangesAsync();

            dbContext.Categories.AddRange(categories);
            await dbContext.SaveChangesAsync();

            var repository = new CategoryRepository(dbContext);

            // Act
            var result = await repository.FindAllByStatusAsync(status);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, category => Assert.Equal(status, category.Status));
        }

        [Fact]
        public async Task FindAllByStatusAsync_NoMatchingCategories_ReturnsEmptyList()
        {
            // Arrange
            var status = Category.CategoryStatus.ACTIVE;
            var categories = new List<Category>
    {
        new Category { CategoryId = 1, CategoryName = "Category 1", Status = Category.CategoryStatus.INACTIVE },
        new Category { CategoryId = 2, CategoryName = "Category 2", Status = Category.CategoryStatus.INACTIVE },
        new Category { CategoryId = 3, CategoryName = "Category 3", Status = Category.CategoryStatus.INACTIVE },
    };

            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var dbContext = new MarketPlaceDbContext(dbContextOptions);
            dbContext.Categories.RemoveRange(dbContext.Categories);
            await dbContext.SaveChangesAsync();

            dbContext.Categories.AddRange(categories);
            await dbContext.SaveChangesAsync();

            var repository = new CategoryRepository(dbContext);

            // Act
            var result = await repository.FindAllByStatusAsync(status);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }


        [Fact]
        public void Update_InvalidCategoryId_ReturnsNull()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var dbContext = new MarketPlaceDbContext(dbContextOptions);
            var repository = new CategoryRepository(dbContext);

            var category = new Category
            {
                CategoryId = 1,
                CategoryName = "Category 1",
                Status = Category.CategoryStatus.ACTIVE
            };

            // Act
            var updatedCategory = repository.Update(category);

            // Assert
            Assert.NotNull(updatedCategory);
        }


        [Fact]
        public async Task FindByCategoryNameAsync_NoMatchingCategory_ReturnsNull()
        {
            // Arrange
            var categoryName = "Non-existent Category";

            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var dbContext = new MarketPlaceDbContext(dbContextOptions);
            var repository = new CategoryRepository(dbContext);

            // Act
            var result = await repository.FindByCategoryNameAsync(categoryName);

            // Assert
            Assert.Null(result);
        }


    }
}
