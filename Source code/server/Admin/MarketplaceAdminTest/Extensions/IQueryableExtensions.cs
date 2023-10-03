using MarketPlace.DataAccess.Extensions;
using System.Linq.Expressions;
using Xunit;

namespace MarketplaceAdminTest.Extensions
{
    public class IQueryableExtensionsTests
    {
        public class MyEntity
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public int Age { get; set; }
        }

        [Fact]
        public void ApplyOrdering_WithValidSortBy_ReturnsOrderedQuery()
        {
            // Arrange
            var query = new List<MyEntity>()
            {
                new MyEntity() { Id = 1, Name = "Alice", Age = 20 },
                new MyEntity() { Id = 2, Name = "Bob", Age = 25 },
                new MyEntity() { Id = 3, Name = "Charlie", Age = 30 }
            }
            .AsQueryable();

            var sortBy = "Name";
            var sortByDesc = false;
            var columnsMap = new Dictionary<string, Expression<Func<MyEntity, object>>>()
            {
                { "Id", x => x.Id },
                { "Name", x => x.Name },
                { "Age", x => x.Age }
            };

            // Act
            var result = query.ApplyOrdering(sortBy, sortByDesc, columnsMap).ToList();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("Alice", result[0].Name);
            Assert.Equal("Bob", result[1].Name);
            Assert.Equal("Charlie", result[2].Name);
        }

        [Fact]
        public void ApplyOrdering_WithInvalidSortBy_ReturnsOriginalQuery()
        {
            // Arrange
            var query = new List<MyEntity>()
            {
                new MyEntity() { Id = 1, Name = "Alice", Age = 20 },
                new MyEntity() { Id = 2, Name = "Bob", Age = 25 },
                new MyEntity() { Id = 3, Name = "Charlie", Age = 30 }
            }
            .AsQueryable();

            var sortBy = "InvalidColumn";
            var sortByDesc = false;
            var columnsMap = new Dictionary<string, Expression<Func<MyEntity, object>>>()
            {
                { "Id", x => x.Id },
                { "Name", x => x.Name },
                { "Age", x => x.Age }
            };

            // Act
            var result = query.ApplyOrdering(sortBy, sortByDesc, columnsMap).ToList();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("Alice", result[0].Name);
            Assert.Equal("Bob", result[1].Name);
            Assert.Equal("Charlie", result[2].Name);
        }

        [Fact]
        public void ApplyOrdering_WithSortByDesc_ReturnsDescendingOrder()
        {
            // Arrange
            var query = new List<MyEntity>()
            {
                new MyEntity() { Id = 1, Name = "Alice", Age = 20 },
                new MyEntity() { Id = 2, Name = "Bob", Age = 25 },
                new MyEntity() { Id = 3, Name = "Charlie", Age = 30 }
            }
            .AsQueryable();

            var sortBy = "Age";
            var sortByDesc = true;
            var columnsMap = new Dictionary<string, Expression<Func<MyEntity, object>>>()
            {
                { "Id", x => x.Id },
                { "Name", x => x.Name },
                { "Age", x => x.Age }
            };

            // Act
            var result = query.ApplyOrdering(sortBy, sortByDesc, columnsMap).ToList();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal(30, result[0].Age);
            Assert.Equal(25, result[1].Age);
            Assert.Equal(20, result[2].Age);
        }
    }


}
