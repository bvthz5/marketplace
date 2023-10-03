using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Enums;
using MarketPlaceAdmin.Bussiness.Interfaces;
using MarketPlaceAdmin.Bussiness.Security.Interfaces;
using MarketPlaceAdmin.Bussiness.Services;
using NSubstitute;
using System.Linq.Expressions;
using Xunit;

namespace MarketplaceAdminTest.Service
{
    public class AgentOrderServiceTests
    {
        private readonly IUnitOfWork _uow;
        private readonly ISecurityUtil _securityUtil;
        private readonly IEmailService _emailService;

        private readonly AgentOrderService _service;

        public AgentOrderServiceTests()
        {
            _uow = Substitute.For<IUnitOfWork>();
            _securityUtil = Substitute.For<ISecurityUtil>();
            _emailService = Substitute.For<IEmailService>();

            _service = new(_uow, _securityUtil, _emailService);
        }

        [Fact]
        public async Task GetOrderList_Invalid_Status()
        {
            // Arrange

            AgentOrderPaginationParams form = new() { OrderStatus = new() { 20 } };

            // Act

            var result = await _service.GetOrderList(form);


            // Assert

            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);

        }

        [Fact]
        public async Task GetOrderList_Invalid_SortBy()
        {
            // Arrange

            AgentOrderPaginationParams form = new() { SortBy = "invalid data" };

            _uow.OrderRepository.AgentColumnMapForSortBy.Returns(new Dictionary<string, Expression<Func<Orders, object>>>()
            {
                ["OrderDate"] = orders => orders.OrderDate,
            });

            // Act

            var result = await _service.GetOrderList(form);


            // Assert

            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);

        }


        [Fact]
        public async Task GetOrderList_Valid_SortBy()
        {
            // Arrange

            AgentOrderPaginationParams form = new()
            {
                SortBy = null,
                Search = "123456",
                MyProductsOnly = true,
                SortByDesc = true,
                OrderStatus = null
            };

            _securityUtil.GetCurrentUserId().Returns(1);


            _uow.OrderRepository.FindByZipcodeAndOrderStatusInAndAgentIdOrderBy(form.Search, Arg.Any<Orders.OrdersStatus[]>(), 1, form.MyProductsOnly, form.SortBy, form.SortByDesc)
                .Returns(Task.FromResult(new List<Orders>() { new Orders() { AgentId = 1 } }));

            // Act

            var result = await _service.GetOrderList(form);


            // Assert

            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);

        }

        //[Fact]
        //public async Task GetOrderList_Success2()
        //{
        //    // Arrange

        //    AgentOrderPaginationParams form = new()
        //    {
        //        SortBy = null,
        //        Search = null,
        //        MyProductsOnly = true,
        //        SortByDesc = true,
        //        OrderStatus = new List<byte?> { (byte)Orders.OrdersStatus.CONFIRMED }
        //    };

        //    _securityUtil.GetCurrentUserId().Returns(1);


        //    _uow.OrderRepository.FindByZipcodeAndOrderStatusInAndAgentIdOrderBy(form.Search, Arg.Any<Orders.OrdersStatus[]>(), (byte)Orders.OrdersStatus.CONFIRMED, form.MyProductsOnly, form.SortBy, form.SortByDesc)
        //        .Returns(Task.FromResult(new List<Orders>() { new Orders() { AgentId = 1, OrderDetails = new List<OrderDetails>() { new OrderDetails() } } }));

        //    // Act

        //    var result = await _service.GetOrderList(form);


        //    // Assert

        //    Assert.NotNull(result);
        //    Assert.Equal(ServiceStatus.Success, result.ServiceStatus);

        //}

        [Fact]
        public async Task AssignOrder_OrderNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            int orderId = 123;
            var uow = Substitute.For<IUnitOfWork>();
            uow.OrderRepository.FindByOrderIdAsync(orderId).Returns(Task.FromResult<Orders?>(null));
            var securityUtil = Substitute.For<ISecurityUtil>();
            var service = new AgentOrderService(uow, securityUtil, _emailService);

            // Act
            var result = await service.AssignOrder(orderId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Order Not Found", result.Message);
        }

        [Fact]
        public async Task AssignOrder_OrderNotConfirmed_ReturnsNotFoundResult()
        {
            // Arrange
            int orderId = 123;
            var order = new Orders { OrdersId = orderId, OrderStatus = Orders.OrdersStatus.CANCELLED };
            var uow = Substitute.For<IUnitOfWork>();
            uow.OrderRepository.FindByOrderIdAsync(orderId).Returns(Task.FromResult<Orders?>(order));
            var securityUtil = Substitute.For<ISecurityUtil>();
            var service = new AgentOrderService(uow, securityUtil, _emailService);

            // Act
            var result = await service.AssignOrder(orderId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Order Not Found", result.Message);
        }

        [Fact]
        public async Task AssignOrder_MaxLimitExceeded_ReturnsNotFoundResult()
        {
            // Arrange
            int orderId = 123;
            var order = new Orders { OrdersId = orderId, OrderStatus = Orders.OrdersStatus.CONFIRMED };
            var uow = Substitute.For<IUnitOfWork>();
            uow.OrderRepository.FindByOrderIdAsync(orderId).Returns(Task.FromResult<Orders?>(order));
            var securityUtil = Substitute.For<ISecurityUtil>();
            securityUtil.GetCurrentUserId().Returns(1);
            uow.OrderRepository.FindOrdersByAgentIdandStatusIn(1, Arg.Any<Orders.OrdersStatus[]>())
                .Returns(Task.FromResult(new List<Orders> { new Orders(), new Orders(), new Orders(), new Orders(), new Orders(), new Orders(), new Orders(), new Orders(), new Orders(), new Orders() }));
            var service = new AgentOrderService(uow, securityUtil, _emailService);

            // Act
            var result = await service.AssignOrder(orderId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Max Limit (10) exceed", result.Message);
        }

        [Fact]
        public async Task AssignOrder_ValidOrder_ReturnsSuccessResult()
        {
            // Arrange
            int orderId = 123;
            var orderDetails = new List<OrderDetails> { new OrderDetails { Histories = new List<OrderHistory>() { new OrderHistory() { Status = OrderHistory.HistoryStatus.CONFIRMED } } } };
            var order = new Orders { OrdersId = orderId, OrderStatus = Orders.OrdersStatus.CONFIRMED, OrderDetails = orderDetails };
            var uow = Substitute.For<IUnitOfWork>();
            uow.OrderRepository.FindByOrderIdAsync(orderId).Returns(Task.FromResult<Orders?>(order));
            var securityUtil = Substitute.For<ISecurityUtil>();
            securityUtil.GetCurrentUserId().Returns(1);
            uow.OrderRepository.FindOrdersByAgentIdandStatusIn(1, Arg.Any<Orders.OrdersStatus[]>())
                .Returns(Task.FromResult(new List<Orders> { new Orders() }));
            var service = new AgentOrderService(uow, securityUtil, _emailService);

            // Act
            var result = await service.AssignOrder(orderId);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Agent Assigned", result.Message);

        }

        [Fact]
        public async Task UnAssignOrder_ShouldReturnNotFound_WhenOrderNotFound()
        {
            // Arrange
            int orderId = 1;
            _uow.OrderRepository.FindByOrderIdAsync(orderId).Returns(Task.FromResult<Orders?>(null));

            // Act
            var result = await _service.UnAssignOrder(orderId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Order Not Found", result.Message);
        }

        [Fact]
        public async Task UnAssignOrder_ValidOrder_ReturnsSuccessResult()
        {
            // Arrange
            int orderId = 123;
            var orderDetails = new List<OrderDetails> { new OrderDetails { Histories = new List<OrderHistory>() { new OrderHistory() { Status = OrderHistory.HistoryStatus.CONFIRMED } } } };
            var order = new Orders { OrdersId = orderId, OrderStatus = Orders.OrdersStatus.WAITING_FOR_PICKUP, AgentId = 1, OrderDetails = orderDetails };
            _securityUtil.GetCurrentUserId().Returns(1);
            _uow.OrderRepository.FindByOrderIdAsync(orderId).Returns(Task.FromResult<Orders?>(order));
            _uow.OrderDetailsRepository.FindByOrderIdAndOrderDetailStatus(order.OrdersId, OrderHistory.HistoryStatus.WAITING_FOR_PICKUP).Returns(orderDetails);

            // Act
            var result = await _service.UnAssignOrder(orderId);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Agent Removed", result.Message);
        }

        [Fact]
        public async Task GetOrderDetails_ShouldReturnNotFound_WhenOrderNotFound()
        {
            // Arrange
            int orderId = 123;
            int agentId = 1;
            _uow.OrderRepository.FindByOrderIdAsync(orderId).Returns(Task.FromResult<Orders?>(null));
            // Act
            var result = await _service.GetOrderDetails(orderId, agentId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Order Not Found", result.Message);
        }

        //[Fact]
        //public async Task GetOrderDetails_ShouldReturnNotFound_WhenAgentIdIsDifferent()
        //{
        //    // Arrange
        //    int orderId = 123;
        //    int agentId = 1;
        //    var order = new Orders { OrdersId = orderId, OrderStatus = Orders.OrdersStatus.CANCELLED, AgentId = 2 };
        //    _securityUtil.GetCurrentUserId().Returns(1);
        //    _uow.OrderRepository.FindByOrderIdAsync(orderId).Returns(Task.FromResult<Orders?>(order));
        //    // Act
        //    var result = await _service.GetOrderDetails(orderId, agentId);

        //    // Assert
        //    Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
        //    Assert.Equal("Order Not Found", result.Message);
        //}

        //[Fact]
        //public async Task GetOrderDetails_ShouldReturnResult()
        //{
        //    // Arrange
        //    int orderId = 123;
        //    int agentId = 1;
        //    Photos photos = new Photos();
        //    var photosList = new List<Photos> { photos };
        //    var order = new Orders { OrdersId = orderId, UserId = 1, User = new User() { UserId = 1 }, OrderStatus = Orders.OrdersStatus.WAITING_FOR_PICKUP, AgentId = 1, DeliveryAddress = "data1\ndata2\ndata3\ndata4\ndata5\ndata6\ndata7" };
        //    var orderDetailsList = new List<OrderDetails>
        //    {
        //        new OrderDetails
        //        {
        //            OrderDetailsId = 1,
        //            OrderId = 123,
        //            ProductId = 123,
        //            Status = 0,
        //            Product = new Product
        //            {
        //                ProductId = 123,
        //                ProductName ="",
        //                ProductDescription = "",
        //                Price = 2400,
        //                CategoryId = 1 ,
        //                Longitude = 90,
        //                Latitude = 90,
        //                Address ="",
        //                Photos =  photosList,
        //                Category = new Category
        //                {
        //                    CategoryName = ""
        //                },
        //                CreatedUser = new User(),
        //                CreatedUserId =1
        //            },
        //            Order = order,
        //            CancellationReason = "reason"
        //        },
        //        new OrderDetails
        //        {
        //            OrderDetailsId = 2,
        //            OrderId = 13,
        //            ProductId = 13,
        //            Status = 1,
        //            Product = new Product
        //            {
        //                ProductId = 13,
        //                ProductName ="",
        //                ProductDescription = "",
        //                Price = 200,
        //                CategoryId = 2 ,
        //                Longitude = 80,
        //                Latitude = 80,
        //                Address ="",
        //                Photos =  photosList,
        //                Category = new Category
        //                {
        //                    CategoryName = ""
        //                },
        //                CreatedUser = new User(),
        //                CreatedUserId =2
        //            },
        //            Order = order
        //        }
        //    };
        //    _securityUtil.GetCurrentUserId().Returns(1);
        //    _uow.OrderRepository.FindByOrderIdAsync(orderId).Returns(Task.FromResult<Orders?>(order));
        //    _uow.OrderDetailsRepository.FindByOrderId(orderId).Returns(orderDetailsList);
        //    // Act
        //    var result = await _service.GetOrderDetails(orderId, agentId);

        //    // Assert
        //    Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
        //    Assert.Equal("Order Detail View", result.Message);
        //}

        [Fact]
        public async Task GetOrderDetails_ShouldReturnResult_WhenAgentIdISDifferernt()
        {
            // Arrange
            int orderId = 123;
            int agentId = 1;
            Photos photos = new Photos();
            var photosList = new List<Photos> { photos };
            var order = new Orders { OrdersId = orderId, UserId = 1, User = new User() { UserId = 1 }, OrderStatus = Orders.OrdersStatus.CONFIRMED, AgentId = null, DeliveryAddress = "data1\ndata2\ndata3\ndata4\ndata5\ndata6\ndata7" };
            var orderDetailsList = new List<OrderDetails>
            {
                new OrderDetails
                {
                    OrderDetailsId = 1,
                    OrderId = 123,
                    ProductId = 123,
                    Histories = new List<OrderHistory>() { new OrderHistory() { Status = OrderHistory.HistoryStatus.CONFIRMED } },
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
                    Order = order
                },
                new OrderDetails
                {
                    OrderDetailsId = 2,
                    OrderId = 13,
                    ProductId = 13,
                    Histories = new List<OrderHistory>() { new OrderHistory() { Status = OrderHistory.HistoryStatus.DELIVERED } },
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
                        CreatedUserId =2
                    },
                    Order = order
                }
            };

            _uow.OrderRepository.FindByOrderIdAsync(orderId).Returns(Task.FromResult<Orders?>(order));
            _uow.OrderDetailsRepository.FindByOrderId(orderId).Returns(orderDetailsList);
            // Act
            var result = await _service.GetOrderDetails(orderId, agentId);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Order Detail View", result.Message);
        }

        //[Fact]
        //public async Task UnAssignOrder_ShouldReturnInvalidStatus_WhenInvalidStatusIsProvided()
        //{
        //    // Arrange
        //    int orderId = 123;
        //    int agentId = 1;
        //    _securityUtil.GetCurrentUserId().Returns(1);


        //    // Act
        //    var result = await _service.ChangeDeliveryStatus(orderId, 1, agentId);

        //    // Assert
        //    Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
        //    Assert.Equal("Invalid Status", result.Message);
        //}

        [Fact]
        public async Task UnAssignOrder_ReturnsNotFoundResult_WhenOrderIsNotFound()
        {
            // Arrange
            int orderId = 123;
            int agentId = 1;
            _securityUtil.GetCurrentUserId().Returns(1);
            _uow.OrderRepository.FindByOrderIdAsync(orderId).Returns(Task.FromResult<Orders?>(null));

            // Act
            var result = await _service.ChangeDeliveryStatus(orderId, 5, agentId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Order Not Found", result.Message);
        }

        //[Fact]
        //public async Task UnAssignOrder_ReturnsSuccessResult()
        //{
        //    // Arrange
        //    int orderId = 123;
        //    int agentId = 1;
        //    var order = new Orders { OrdersId = orderId, OrderStatus = Orders.OrdersStatus.WAITING_FOR_PICKUP, AgentId = 1 };
        //    var orderDetails = new List<OrderDetails> { new OrderDetails { Status = OrderDetails.OrderDetailsStatus.WAITING_FOR_PICKUP } };

        //    _securityUtil.GetCurrentUserId().Returns(1);
        //    _uow.OrderRepository.FindByOrderIdAsync(orderId).Returns(Task.FromResult<Orders?>(order));
        //    _uow.OrderDetailsRepository.FindByOrderIdAndOrderDetailStatus(order.OrdersId, OrderDetails.OrderDetailsStatus.WAITING_FOR_PICKUP).Returns(orderDetails);

        //    // Act
        //    var result = await _service.ChangeDeliveryStatus(orderId, 5, agentId);

        //    // Assert
        //    Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
        //    Assert.Equal($"Status Changed to {Orders.OrdersStatus.INTRANSIT}", result.Message);
        //}

    }
}