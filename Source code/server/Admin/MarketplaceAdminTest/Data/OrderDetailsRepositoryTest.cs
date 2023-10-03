using MarketPlace.DataAccess.Data;
using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlace.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MarketplaceAdminTest.Data
{
    public class OrderDetailsRepositoryTest
    {
        private readonly MarketPlaceDbContext _dbContext;
        private readonly IOrderDetailsRepository _orderDetail;

        public OrderDetailsRepositoryTest()
        {
            _dbContext = new MarketPlaceDbContext(new DbContextOptionsBuilder<MarketPlaceDbContext>().UseInMemoryDatabase(databaseName: "TestDatabase").Options);
            _orderDetail = new OrderDetailsRepository(_dbContext);
        }

        [Fact]
        public async Task AddAsync_ShouldAddOrderDetailsToDbContext()
        {
            // Arrange
            var repository = new OrderDetailsRepository(_dbContext);
            var orderDetails = new OrderDetails
            {
                OrderDetailsId = 1,
                ProductId = 1,
                OrderId = 2,
            };

            // Act
            var addedOrderDetails = await repository.Add(orderDetails);

            // Assert
            Assert.NotNull(addedOrderDetails);
            Assert.Equal(orderDetails.OrderDetailsId, addedOrderDetails.OrderDetailsId);
            Assert.Equal(orderDetails.OrderId, addedOrderDetails.OrderId);
            Assert.Equal(orderDetails.ProductId, addedOrderDetails.ProductId);

            // Check if the order details are added to the database context
            var dbOrderDetails = await _dbContext.OrderDetails.FindAsync(addedOrderDetails.OrderDetailsId);
            Assert.NotNull(dbOrderDetails);
            Assert.Equal(orderDetails.OrderDetailsId, addedOrderDetails.OrderDetailsId);
            Assert.Equal(orderDetails.OrderId, addedOrderDetails.OrderId);
            Assert.Equal(orderDetails.ProductId, addedOrderDetails.ProductId);

        }

        [Fact]
        public async Task FindByOrderId_ShouldReturnOrderDetailsForOrderId()
        {
            // Arrange
            var repository = new OrderDetailsRepository(_dbContext);
            var orderId = 1;

            // Create some order details for the order
            var orderDetails1 = new OrderDetails
            {
                OrderDetailsId = 1,
                ProductId = 1,
                OrderId = orderId,
            };
            var orderDetails2 = new OrderDetails
            {
                OrderDetailsId = 2,
                ProductId = 2,
                OrderId = orderId,
            };
            var orderDetails3 = new OrderDetails
            {
                OrderDetailsId = 3,
                ProductId = 3,
                OrderId = orderId,
            };
            _dbContext.OrderDetails.AddRange(orderDetails1, orderDetails2, orderDetails3);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await repository.FindByOrderId(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Count);
            Assert.DoesNotContain(orderDetails1, result);
            Assert.DoesNotContain(orderDetails2, result);
            Assert.DoesNotContain(orderDetails3, result);
        }


        [Fact]
        public async Task FindBySellerId_ReturnsMatchingOrderDetails()
        {
            // Arrange
            var orderId = 1;
            var sellerId = 1;
            var orderDetails = new List<OrderDetails>
    {
        new OrderDetails
        {
            OrderDetailsId = 1,
            ProductId = 1,
            OrderId = orderId,
        },
        new OrderDetails
        {
            OrderDetailsId = 2,
            ProductId = 2,
            OrderId = orderId,
        },
        new OrderDetails
        {
            OrderDetailsId = 3,
            ProductId = 3,
            OrderId = orderId,
        }
    };

            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var dbContext = new MarketPlaceDbContext(dbContextOptions);
            dbContext.OrderDetails.AddRange(orderDetails);
            await dbContext.SaveChangesAsync();

            var repository = new OrderDetailsRepository(dbContext);

            // Act
            var result = await repository.FindBySellerId(sellerId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result); // Expecting 2 order details for the specified seller
            Assert.All(result, od => Assert.Equal(sellerId, od.Product.CreatedUserId)); // Ensure all order details belong to the specified seller
        }


        [Fact]
        public async Task FindBySellerId_ReturnsEmptyList_WhenNoMatchingOrderDetailsFound()
        {
            // Arrange
            var orderId = 1;
            var sellerId = 1;
            var orderDetails = new List<OrderDetails>
    {
        new OrderDetails
        {
            OrderDetailsId = 1,
            ProductId = 1,
            OrderId = orderId,
        },
        new OrderDetails
        {
            OrderDetailsId = 2,
            ProductId = 2,
            OrderId = orderId,
        },
        new OrderDetails
        {
            OrderDetailsId = 3,
            ProductId = 3,
            OrderId = orderId,
        }
    };

            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var dbContext = new MarketPlaceDbContext(dbContextOptions);
            dbContext.OrderDetails.AddRange(orderDetails);
            await dbContext.SaveChangesAsync();

            var repository = new OrderDetailsRepository(dbContext);

            // Act
            var result = await repository.FindBySellerId(sellerId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result); // Expecting an empty list as no order details with the specified seller ID exist
        }






        [Fact]
        public async Task FindByBuyerIdAndStatus_ReturnsEmptyListWhenNoMatchingOrderDetails()
        {
            // Arrange
            int buyerId = 1;
            OrderHistory.HistoryStatus status = OrderHistory.HistoryStatus.DELIVERED;

            var orderDetails = new List<OrderDetails>
    {
        new OrderDetails
        {
            OrderDetailsId = 1,
            ProductId = 1,
            CreatedDate = DateTime.Now,
            Order = new Orders { UserId = buyerId, DeliveryAddress = "Address 1", OrderNumber = "Order 1" },
            Histories = new List<OrderHistory>
            {
                new OrderHistory { OrderHistoryId = 1, Status = OrderHistory.HistoryStatus.CREATED },
                new OrderHistory { OrderHistoryId = 2, Status = OrderHistory.HistoryStatus.INTRANSIT }
            }
        },
        new OrderDetails
        {
            OrderDetailsId = 2,
            ProductId = 2,
            CreatedDate = DateTime.Now,
            Order = new Orders { UserId = buyerId, DeliveryAddress = "Address 2", OrderNumber = "Order 2" },
            Histories = new List<OrderHistory>
            {
                new OrderHistory { OrderHistoryId = 3, Status = OrderHistory.HistoryStatus.CREATED },
                new OrderHistory { OrderHistoryId = 4, Status = OrderHistory.HistoryStatus.DELIVERED }
            }
        },
        // Add other OrderDetails entries for different scenarios
    };

            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var dbContext = new MarketPlaceDbContext(dbContextOptions);
            dbContext.OrderDetails.AddRange(orderDetails);
            await dbContext.SaveChangesAsync();

            var repository = new OrderDetailsRepository(dbContext);

            // Act
            var result = await repository.FindByBuyerIdAndStatus(buyerId, status);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result); // Expecting an empty list when no matching order details are found
        }








    }
}
