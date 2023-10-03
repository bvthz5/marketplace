using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Dto.Views;
using MarketPlaceAdmin.Bussiness.Enums;
using MarketPlaceAdmin.Bussiness.Services;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System.Linq.Expressions;
using Xunit;

namespace MarketplaceAdminTest.Service
{
    public class OrderServiceTests
    {
        private readonly IUnitOfWork _uow;
        private readonly AdminOrderService _orderService;

        public OrderServiceTests()
        {
            // Create a new instance of the IUnitOfWork interface using NSubstitute
            _uow = Substitute.For<IUnitOfWork>();
            _orderService = new AdminOrderService(_uow);

        }

        [Fact]
        public async Task GetOrderDetails_WithValidOrderId_ReturnsOrderDetails()
        {
            // Arrange
            int orderId = 123;

            Photos photos = new Photos();
            var photosList = new List<Photos> { photos };
            var orderDetailsList = new List<OrderDetails>
            {
                new OrderDetails
                {
                    OrderDetailsId = 1,
                    OrderId = 123,
                    ProductId = 123,
                    Histories = new List < OrderHistory >() { new OrderHistory() { Status = OrderHistory.HistoryStatus.CONFIRMED } },
                    Product = new Product
                    {
                        ProductId = 123,
                        ProductName ="",
                        ProductDescription = "",
                        Price = 2400,
                        CategoryId = 1 ,
                        Longitude = 90,
                        Latitude = 90,
                        Address ="",
                        Photos =  photosList,
                        Category = new Category
                        {
                            CategoryName = ""
                        },
                        CreatedUser = new User(),
                        CreatedUserId =1
                    },
                },
                new OrderDetails
                {
                    OrderDetailsId = 2,
                    OrderId = 13,
                    ProductId = 13,
                    Histories = new List<OrderHistory>() { new OrderHistory() { Status = OrderHistory.HistoryStatus.CONFIRMED } },
                    Product = new Product
                    {
                        ProductId = 13,
                        ProductName ="",
                        ProductDescription = "",
                        Price = 200,
                        CategoryId = 2 ,
                        Longitude = 80,
                        Latitude = 80,
                        Address ="",
                        Photos =  photosList,
                        Category = new Category
                        {
                            CategoryName = ""
                        },
                        CreatedUser = new User(),
                        CreatedUserId = 2
                    },
                }
            };
            var order = new Orders { OrdersId = orderId, UserId = 1, OrderDetails = orderDetailsList, User = new User() { UserId = 1, Email = "Buyer Email" }, DeliveryAddress = "data1\ndata2\ndata3\ndata4\ndata5\ndata6\ndata7" };
            // Set up the OrderRepository to return the mock order object
            _uow.OrderRepository.FindByOrderIdAsync(orderId).Returns(order);

            // Set up the OrderDetailsRepository to return the mock order details list
            _uow.OrderDetailsRepository.FindByOrderId(orderId).Returns(orderDetailsList);

            // Act
            var result = await _orderService.GetOrderDetails(orderId);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Order Detail View", result.Message);
            Assert.NotNull(result.Data);

            var orderDetailView = result.Data as OrderDetailView;
            Assert.NotNull(orderDetailView);
            Assert.Equal(orderDetailsList.Count, orderDetailView.Items.Count());

            for (int i = 0; i < orderDetailsList.Count; i++)
            {
                Assert.Equal(orderDetailsList[i].OrderDetailsId, orderDetailView.Items.ElementAt(i).OrderDetailsId);
                Assert.Equal((byte)orderDetailsList[i].Histories.Last().Status, orderDetailView.Items.ElementAt(i).Status);

                Assert.Equal(order.OrdersId, orderDetailView.OrdersId);
                Assert.Equal((byte)order.OrderStatus, orderDetailView.OrderStatus);
                Assert.Equal(order.OrderNumber, orderDetailView.OrderNumber);
                Assert.Equal((byte)order.PaymentStatus, orderDetailView.PaymentStatus);
                Assert.Equal(order.OrderDate, orderDetailView.OrderDate);
                Assert.Equal(order.PaymentDate, orderDetailView.PaymentDate);
                Assert.Equal(order.TotalPrice, orderDetailView.TotalPrice);
                Assert.Equal(order.User.Email, orderDetailView.Buyer.Email);
                Assert.NotNull(orderDetailView.DeliveryAddress);

                var productView = orderDetailView.Items.ElementAt(i).Product;
                Assert.NotNull(productView);
                Assert.Equal(orderDetailsList[i].ProductId, productView.ProductId);

            }
        }

        [Fact]
        public async Task GetOrderDetails_WithInvalidOrderId_ReturnsNotFoundStatus()
        {
            // Arrange
            int orderId = 456;

            _uow.OrderRepository.FindByOrderIdAsync(orderId).ReturnsNull();

            // Act
            var result = await _orderService.GetOrderDetails(orderId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Order Not Found", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetOrderDetails_OrderDetailsNotGenerated_ReturnsNotFoundStatus()
        {
            // Arrange
            int orderId = 456;

            var service = new AdminOrderService(_uow);

            _uow.OrderRepository.FindByOrderIdAsync(orderId).Returns(new Orders() { OrderDetails = new List<OrderDetails>() });

            // Act
            var result = await service.GetOrderDetails(orderId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Order Details Not Generated", result.Message);
            Assert.Null(result.Data);
        }


        [Fact]
        public async Task GetOrderList_WithValidParams_ReturnsPaginatedOrderList()
        {
            // Arrange
            var form = new OrderPaginationParams
            {
                Search = "test",
                SortBy = "OrderDate",
                SortByDesc = true,
                PageNumber = 1,
                PageSize = 10,
                BuyerId = 1,
                PaymentStatus = new List<byte?>() { (byte)Orders.PaymentsStatus.PAID },
                OrderStatus = new List<byte?>() { (byte)Orders.OrdersStatus.CREATED }
            };

            _uow.OrderRepository.ColumnMapForSortBy.Returns(new Dictionary<string, Expression<Func<Orders, object>>>()
            {
                ["OrdersId"] = orders => orders.OrdersId,
                ["Price"] = orders => orders.TotalPrice,
                ["OrderDate"] = orders => orders.OrderDate,
                ["PaymentDate"] = orders => orders.PaymentDate
            });
            var orders = new List<Orders>
            {
               new Orders { OrdersId = 1, User = new User(), DeliveryAddress = "" ,OrderNumber= "",OrderStatus = Orders.OrdersStatus.CANCELLED,PaymentStatus= Orders.PaymentsStatus.PAID},
               new Orders { OrdersId = 2, User = new User(), DeliveryAddress = "", OrderNumber= "",OrderStatus = Orders.OrdersStatus.CONFIRMED,PaymentStatus= Orders.PaymentsStatus.UNPPAID }
            };

            _uow.OrderRepository.FindAll(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<Orders.PaymentsStatus[]>(), Arg.Any<int>(), Arg.Any<Orders.OrdersStatus[]>())
                .Returns(Task.FromResult(orders));

            // Act
            var result = await _orderService.GetOrderList(form);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Paginated Order List", result.Message);
            Assert.IsType<Pager<OrderView>>(result.Data);

            var paginatedOrders = (Pager<OrderView>)result.Data;
            Assert.Equal(2, paginatedOrders.TotalItems);
            Assert.Equal(1, paginatedOrders.TotalPages);
            Assert.Equal(10, paginatedOrders.PageSize);
            Assert.Equal(2, paginatedOrders.TotalItems);
        }

        [Fact]
        public async Task GetOrderList_WithInvalidPaymentStatus_ReturnsBadRequest()
        {
            // Arrange
            var form = new OrderPaginationParams
            {
                PaymentStatus = new List<byte?>() { 100 }
            };

            // Act
            var result = await _orderService.GetOrderList(form);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid PaymentStatus Value", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetOrderList_WithInvalidOrderStatus_ReturnsBadRequest()
        {
            // Arrange
            var form = new OrderPaginationParams
            {
                OrderStatus = new List<Byte?>() { 10 }
            };

            // Act
            var result = await _orderService.GetOrderList(form);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid OrderStatus Value", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetOrderList_WithInvalidSortBy_ReturnsBadRequest()
        {
            // Arrange
            var form = new OrderPaginationParams
            {
                SortBy = "InvalidSortBy"
            };

            _uow.OrderRepository.ColumnMapForSortBy.Returns(new Dictionary<string, Expression<Func<Orders, object>>>()
            {
                ["OrdersId"] = orders => orders.OrdersId,
                ["Price"] = orders => orders.TotalPrice,
                ["OrderDate"] = orders => orders.OrderDate,
                ["PaymentDate"] = orders => orders.PaymentDate
            });

            // Act
            var result = await _orderService.GetOrderList(form);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("SortBy : Accepts [OrdersId, Price, OrderDate, PaymentDate] values only", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetOrderList_WithNulldSortBy_ReturnsBadRequest()
        {
            // Arrange
            var form = new OrderPaginationParams
            {
                SortBy = null
            };

            _uow.OrderRepository.ColumnMapForSortBy.Returns(new Dictionary<string, Expression<Func<Orders, object>>>()
            {
                ["OrdersId"] = orders => orders.OrdersId,
                ["Price"] = orders => orders.TotalPrice,
                ["OrderDate"] = orders => orders.OrderDate,
                ["PaymentDate"] = orders => orders.PaymentDate
            });

            var orders = new List<Orders>
            {
               new Orders { OrdersId = 1, User = new User(), DeliveryAddress = "" },
               new Orders { OrdersId = 2, User = new User(), DeliveryAddress = "" }
            };

            _uow.OrderRepository.FindAll(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<Orders.PaymentsStatus[]>(), Arg.Any<int>(), Arg.Any<Orders.OrdersStatus[]>())
                .Returns(Task.FromResult(orders));


            // Act
            var result = await _orderService.GetOrderList(form);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Paginated Order List", result.Message);
            Assert.NotNull(result.Data);
        }


        [Fact]
        public async Task GetHistory_NotFound()
        {
            //Arrange

            int orderDetailsId = 1;

            _uow.OrderDetailsRepository.FindByOrderDetailsId(orderDetailsId).ReturnsNull();

            //Act

            var result = await _orderService.GetOrderHistory(orderDetailsId);

            //Assert

            Assert.NotNull(result);
            Assert.False(result.Status);
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetHistory_Success()
        {
            //Arrange

            int orderDetailsId = 1;

            OrderDetails orderDetails = new()
            {
                OrderId = orderDetailsId,
                Histories = new List<OrderHistory> { new() { OrderHistoryId = 10, OrderDetailsId = orderDetailsId, Remark = "", Date = DateTime.Now, Status = OrderHistory.HistoryStatus.CREATED } }
            };

            _uow.OrderDetailsRepository.FindByOrderDetailsId(orderDetailsId).Returns(orderDetails);

            //Act

            var result = await _orderService.GetOrderHistory(orderDetailsId);

            //Assert

            Assert.NotNull(result);
            Assert.True(result.Status);
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.NotNull(result.Data);
            Assert.IsAssignableFrom<IEnumerable<OrderHistoryView>>(result.Data);

            Assert.Single((IEnumerable<OrderHistoryView>)result.Data);
            Assert.Equal(OrderHistory.HistoryStatus.CREATED, ((IEnumerable<OrderHistoryView>)result.Data).ElementAt(0).Status);
        }

    }
}
