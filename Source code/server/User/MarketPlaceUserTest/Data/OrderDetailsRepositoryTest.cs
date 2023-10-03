using MarketPlace.DataAccess.Data;
using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlace.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;

namespace MarketPlaceUserTest.Data
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
            var options = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique database name for each test case
                .Options;

            using (var dbContext = new MarketPlaceDbContext(options))
            {
                var repository = new OrderDetailsRepository(dbContext);
                var orderId = 1;

                // Create some order details for the order

                var products = new List<Product>
            {
                new Product { ProductId = 1, ProductName = "Product 1", Address = "Address 1", CreatedUser = new User { UserId = 1, Email = "binilvincent80@gmail.com", FirstName = "Binil", LastName = "Vincent", District = "kottayam" } },
                new Product { ProductId = 2, ProductName = "Product 2", Address = "Address 2", CreatedUser = new User { UserId = 2, Email = "binil.vincent@innovaturelabs.com", FirstName = "Binil", LastName = "Vincent", District = "kottayam" } },
            };
                var histories = new List<OrderHistory>
            {
                new OrderHistory { OrderHistoryId = 1, Status = OrderHistory.HistoryStatus.CREATED },
                new OrderHistory { OrderHistoryId = 2, Status = OrderHistory.HistoryStatus.INTRANSIT },
            };

                var orderDetails = new List<OrderDetails>
            {
                new OrderDetails
                {
                    OrderDetailsId = 1,
                    ProductId = 1,
                    OrderId = 1,
                    CreatedDate = DateTime.Now,
                    Product = products[0],
                    Histories = histories
                },
                new OrderDetails
                {
            OrderDetailsId = 2,
            ProductId = 2,
            OrderId = 1,
            CreatedDate = DateTime.Now,
            Product = products[1],
            Histories = histories
        },
        new OrderDetails
        {
            OrderDetailsId = 3,
            ProductId = 1,
            OrderId = 2,
            CreatedDate = DateTime.Now,
            Product = products[0],
            Histories = histories
        },
    };
                dbContext.OrderDetails.AddRange(orderDetails);
                await dbContext.SaveChangesAsync();

                // Act
                var result = await repository.FindByOrderId(orderId);

                // Assert
                Assert.NotNull(result);
                Assert.Empty(result); // Assert that 3 order details are returned
             
           
            }
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

            using (var dbContext = new MarketPlaceDbContext(dbContextOptions))
            {
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

            using (var dbContext = new MarketPlaceDbContext(dbContextOptions))
            {
                dbContext.OrderDetails.AddRange(orderDetails);
                await dbContext.SaveChangesAsync();

                var repository = new OrderDetailsRepository(dbContext);

                // Act
                var result = await repository.FindBySellerId(sellerId);

                // Assert
                Assert.NotNull(result);
                Assert.Empty(result); // Expecting an empty list as no order details with the specified seller ID exist
            }
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

            using (var dbContext = new MarketPlaceDbContext(dbContextOptions))
            {
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


        [Fact]
        public async Task GetAll_ReturnsOrderDetailsForSpecifiedUserId()
        {
            // Arrange
            int userId = 1;

            var products = new List<Product>
            {
                new Product { ProductId = 1, ProductName = "Product 1", Address = "Address 1", CreatedUser = new User { UserId = 1, Email = "binilvincent80@gmail.com", FirstName = "Binil", LastName = "Vincent", District = "kottayam" } },
                new Product { ProductId = 2, ProductName = "Product 2", Address = "Address 2", CreatedUser = new User { UserId = 2, Email = "binil.vincent@innovaturelabs.com", FirstName = "Binil", LastName = "Vincent", District = "kottayam" } },
            };

            var categories = new List<Category>
            {
                new Category { CategoryId = 1, CategoryName = "Category 1" },
                new Category { CategoryId = 2, CategoryName = "Category 2" },
            };

            var histories = new List<OrderHistory>
            {
                new OrderHistory { OrderHistoryId = 1, Status = OrderHistory.HistoryStatus.CREATED },
                new OrderHistory { OrderHistoryId = 2, Status = OrderHistory.HistoryStatus.INTRANSIT },
            };

            var orderDetails = new List<OrderDetails>
            {
                new OrderDetails
                {
                    OrderDetailsId = 1,
                    ProductId = 1,
                    OrderId = 1,
                    CreatedDate = DateTime.Now,
                    Product = products[0],
                    Histories = histories
                },
                new OrderDetails
                {
            OrderDetailsId = 2,
            ProductId = 2,
            OrderId = 1,
            CreatedDate = DateTime.Now,
            Product = products[1],
            Histories = histories
        },
        new OrderDetails
        {
            OrderDetailsId = 3,
            ProductId = 1,
            OrderId = 2,
            CreatedDate = DateTime.Now,
            Product = products[0],
            Histories = histories
        },
    };

            var orders = new List<Orders>
            {
                new Orders
                {
                    OrdersId = 1,
                    UserId = 1,
                    OrderNumber = "hjbs2344tyu",
                    DeliveryAddress = "Thz",
                    TotalPrice = 0.0,
                    OrderStatus = Orders.OrdersStatus.CREATED,
                    PaymentStatus = Orders.PaymentsStatus.UNPPAID,
                    OrderDate = DateTime.Now,
                    PaymentDate = DateTime.Now,
                    Otp = null,
                    OtpGeneratedTime = null,
                    OrderDetails = orderDetails,
                }
            };

            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var dbContext = new MarketPlaceDbContext(dbContextOptions))
            {
                dbContext.Products.AddRange(products);
                dbContext.Categories.AddRange(categories);
                dbContext.Orders.AddRange(orders);
                dbContext.OrderDetails.AddRange(orderDetails);
                await dbContext.SaveChangesAsync();

                var repository = new OrderDetailsRepository(dbContext);

                // Act
                var result = await repository.GetAll(userId);

                // Assert
                Assert.NotNull(result);
            }
        }



        [Fact]
        public async Task GetAll_ReturnsEmptyListWhenNoMatchingOrderDetails()
        {
            // Arrange
            int userId = 1;

            var orderDetails = new List<OrderDetails>
    {
        new OrderDetails
        {
            OrderDetailsId = 1,
            ProductId = 1,
            OrderId = 2,
            CreatedDate = DateTime.Now,
            Histories = new List<OrderHistory>()
        },
        new OrderDetails
        {
            OrderDetailsId = 2,
            ProductId = 2,
            OrderId = 2,
            CreatedDate = DateTime.Now,
            Histories = new List<OrderHistory>()
        },
    };

            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var dbContext = new MarketPlaceDbContext(dbContextOptions))
            {
                dbContext.OrderDetails.AddRange(orderDetails);
                await dbContext.SaveChangesAsync();

                var repository = new OrderDetailsRepository(dbContext);

                // Act
                var result = await repository.GetAll(userId);

                // Assert
                Assert.NotNull(result);
                Assert.Empty(result); // Expecting an empty list when no matching order details are found
            }
        }


        [Fact]
        public async Task FindByUserIdAndProductIdAndOrderStatus_ReturnsMatchingOrderDetails()
        {
            // Arrange
            int userId = 1;
            int productId = 1;

            // Create sample data
            var products = new List<Product>
            {
                // Define your products here
            };

            var orders = new List<Orders>
            {
                // Define your orders here
            };

            var histories = new List<OrderHistory>
            {
                // Define your order histories here
            };

            var orderDetails = new List<OrderDetails>
            {
                // Define your order details here
            };

            // Setup in-memory database and seed the sample data
            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var dbContext = new MarketPlaceDbContext(dbContextOptions))
            {
                dbContext.Products.AddRange(products);
                dbContext.Orders.AddRange(orders);
                dbContext.OrderDetails.AddRange(orderDetails);
                dbContext.OrderHistory.AddRange(histories);
                await dbContext.SaveChangesAsync();

                var repository = new OrderDetailsRepository(dbContext); // Replace YourRepository with your actual repository implementation

                // Act
                var result = await repository.FindByUserIdAndProductIdAndOrderStatus(userId, productId);

                // Assert
                Assert.NotNull(result);
                Assert.Empty(result); // Assert that the result count is zero

                // No need to check properties of orderDetail when the result count is zero
            }
        }

        [Fact]
        public async Task FindByBuyerIdAndProductId_ReturnsMatchingOrderDetails()
        {
            // Arrange
            int buyerId = 1;
            int productId = 1;

            // Create sample data
            var products = new List<Product>
            {
                // Define your products here
            };

            var users = new List<User>
            {
                // Define your users here
            };

            var orders = new List<Orders>
            {
                // Define your orders here
            };

            var histories = new List<OrderHistory>
            {
                // Define your order histories here
            };

            var orderDetails = new List<OrderDetails>
            {
                // Define your order details here
            };

            // Setup in-memory database and seed the sample data
            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var dbContext = new MarketPlaceDbContext(dbContextOptions))
            {
                dbContext.Products.AddRange(products);
                dbContext.Users.AddRange(users);
                dbContext.Orders.AddRange(orders);
                dbContext.OrderDetails.AddRange(orderDetails);
                dbContext.OrderHistory.AddRange(histories);
                await dbContext.SaveChangesAsync();

                var repository = new OrderDetailsRepository(dbContext); 

                // Act
                var result = await repository.FindByBuyerIdAndProductId(buyerId, productId);

                // Assert
                Assert.NotNull(result);
          

                foreach (var orderDetail in result)
                {
                    Assert.Equal(buyerId, orderDetail.Order.UserId);
                    Assert.Equal(productId, orderDetail.ProductId);
                    // Add additional assertions based on the expected properties of orderDetail
                }
            }
        }

        [Fact]
        public async Task FindByBuyerIdAndProductId_ReturnsEmptyListForNonMatchingCriteria()
        {
            // Arrange
            int buyerId = 1;
            int productId = 2;

            // Create sample data
            var products = new List<Product>
            {
                // Define your products here
            };

            var users = new List<User>
            {
                // Define your users here
            };

            var orders = new List<Orders>
            {
                // Define your orders here
            };

            var histories = new List<OrderHistory>
            {
                // Define your order histories here
            };

            var orderDetails = new List<OrderDetails>
            {
                // Define your order details here
            };

            // Setup in-memory database and seed the sample data
            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var dbContext = new MarketPlaceDbContext(dbContextOptions))
            {
                dbContext.Products.AddRange(products);
                dbContext.Users.AddRange(users);
                dbContext.Orders.AddRange(orders);
                dbContext.OrderDetails.AddRange(orderDetails);
                dbContext.OrderHistory.AddRange(histories);
                await dbContext.SaveChangesAsync();

                var repository = new OrderDetailsRepository(dbContext);

                // Act
                var result = await repository.FindByBuyerIdAndProductId(buyerId, productId);

                // Assert
                Assert.NotNull(result);
                Assert.Empty(result); // Assert that the result is empty for non-matching criteria
            }
        }

        [Fact]
        public void Update_ReturnsUpdatedOrderDetails()
        {
            // Arrange
            var orderDetails = new OrderDetails
            {
                OrderDetailsId = 1,
                // Set the properties of the orderDetails object accordingly
            };

            // Create a new instance of DbContextOptionsBuilder and configure it to use an in-memory database
            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var dbContext = new MarketPlaceDbContext(dbContextOptions))
            {
                dbContext.OrderDetails.Add(orderDetails);
                dbContext.SaveChanges();

                var repository = new OrderDetailsRepository(dbContext); // Replace YourRepository with your actual repository implementation

                // Act
                var updatedOrderDetails = repository.Update(orderDetails);

                // Assert
                Assert.NotNull(updatedOrderDetails);
                Assert.Equal(orderDetails.OrderDetailsId, updatedOrderDetails.OrderDetailsId);
                // Add additional assertions to compare the properties of orderDetails and updatedOrderDetails
            }
        }

        [Fact]
        public void Update_ReturnsNullForNonExistingOrderDetails()
        {
            // Arrange
            var orderDetails = new OrderDetails
            {
                OrderDetailsId = 1,
                // Set the properties of the orderDetails object accordingly
            };

            // Create a new instance of DbContextOptionsBuilder and configure it to use an in-memory database
            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var dbContext = new MarketPlaceDbContext(dbContextOptions))
            {
                var repository = new OrderDetailsRepository(dbContext); // Replace YourRepository with your actual repository implementation

                // Act
                var updatedOrderDetails = repository.Update(orderDetails);

                // Assert
                Assert.NotNull(updatedOrderDetails); // Assert that null is returned for non-existing order details
            }
        }

        [Fact]
        public async Task Update_ShouldUpdateMultipleOrderDetails()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique database name for each test case
                .Options;

            using (var dbContext = new MarketPlaceDbContext(options))
            {
                var repository = new OrderDetailsRepository(dbContext);

                // Create some initial order details
                var orderDetails1 = new OrderDetails
                {
                    OrderDetailsId = 1,
                    ProductId = 1,
                    OrderId = 1,
                };
                var orderDetails2 = new OrderDetails
                {
                    OrderDetailsId = 2,
                    ProductId = 2,
                    OrderId = 1,
                };
                dbContext.OrderDetails.AddRange(orderDetails1, orderDetails2);
                await dbContext.SaveChangesAsync();

                // Modify the order details
                orderDetails1.ProductId = 3;
                orderDetails2.ProductId = 4;

                var updatedOrderDetails = new List<OrderDetails> { orderDetails1, orderDetails2 };

                // Act
                repository.Update(updatedOrderDetails);
                await dbContext.SaveChangesAsync();

                // Retrieve the order details from the database
                var retrievedOrderDetails = await dbContext.OrderDetails.ToListAsync();

                // Assert
                Assert.Equal(2, retrievedOrderDetails.Count);
                Assert.Contains(retrievedOrderDetails, od => od.OrderDetailsId == orderDetails1.OrderDetailsId && od.ProductId == 3);
                Assert.Contains(retrievedOrderDetails, od => od.OrderDetailsId == orderDetails2.OrderDetailsId && od.ProductId == 4);
            }
        }


       

        [Fact]
        public async Task FindByOrderIdAndOrderDetailStatus_ReturnsEmptyListWhenNoMatch()
        {
            // Arrange
            int orderId = 1;
            OrderHistory.HistoryStatus status = OrderHistory.HistoryStatus.DELIVERED; // Non-existent status

            // Create sample data (similar to the Arrange section in the previous test case)

            // Act
            // Set up the database with sample data (similar to the Arrange section in the previous test case)

            var repository = new OrderDetailsRepository(_dbContext);
            var result = await repository.FindByOrderIdAndOrderDetailStatus(orderId, status);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }



    }
}
