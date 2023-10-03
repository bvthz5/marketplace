using MarketPlace.DataAccess.Extensions;
using MarketPlace.DataAccess.Model;
using NSubstitute;
using System.Linq.Expressions;

namespace MarketPlaceUserTest.Data
{

    public class IQueryableExtensionsTests
    {
       private readonly IQueryable<User> _query ;

       private readonly Dictionary<string, Expression<Func<User, object>>> _columnsMap;

        public IQueryableExtensionsTests()
        {
            // Arrange
            List<User> users = new List<User>
            {
                new User { UserId = 1, FirstName = "John", Email = "john@test.com" },
                new User { UserId = 2, FirstName = "Alice", Email = "alice@test.com" },
                new User { UserId = 3, FirstName = "Bob", Email = "bob@test.com" }
            };

            _columnsMap = new Dictionary<string, Expression<Func<User, object>>>
            {
                { "UserId", u => u.UserId },
                { "FirstName", u => u.FirstName },
                { "Email", u => u.Email }
            };

            _query = users.AsQueryable();

        }

        [Fact]
        public void ApplyOrderingWithValidSortByAscReturnsOrderedQuery()
        {
            // Act
            var result = _query.ApplyOrdering("FirstName", false, _columnsMap).ToList();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("Alice", result[0].FirstName);
            Assert.Equal("Bob", result[1].FirstName);
            Assert.Equal("John", result[2].FirstName);
        }

        [Fact]
        public void ApplyOrdering_WithInvalidSortBy_ReturnsUnorderedQuery()
        {
            // Act
            var result = _query.ApplyOrdering("Invalid", false, _columnsMap).ToList();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("John", result[0].FirstName);
            Assert.Equal("Alice", result[1].FirstName);
            Assert.Equal("Bob", result[2].FirstName);
        }

        [Fact]
        public void ApplyOrdering_WithWhiteSpaceSortBy_ReturnsUnorderedQuery()
        {
            // Act
            var result = _query.ApplyOrdering("", false, _columnsMap).ToList();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("John", result[0].FirstName);
            Assert.Equal("Alice", result[1].FirstName);
            Assert.Equal("Bob", result[2].FirstName);
        }

        [Fact]
        public void ApplyOrdering_WithValidSortByDesc_ReturnsOrderedQuery()
        {
            // Act
            var result = _query.ApplyOrdering("Email", true, _columnsMap).ToList();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("john@test.com", result[0].Email);
            Assert.Equal("bob@test.com", result[1].Email);
            Assert.Equal("alice@test.com", result[2].Email);
        }


    }
}
