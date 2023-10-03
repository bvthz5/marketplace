using MarketPlaceUser.Api.Controllers;
using MarketPlaceUser.Bussiness.Dto.Forms;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;
using MarketPlaceUser.Bussiness.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NSubstitute;
using System.Net;

namespace MarketPlaceUserTest.TestController
{
    public class OrderTestController
    {
        private readonly IOrderService _orderService;
        private readonly ISecurityUtil _securityUtil;
        private readonly IRefundService _refundService;
        private readonly IOrderDetailsService _orderDetailsService;
        private readonly OrderController _controller;

        public OrderTestController()
        {
            _orderService = Substitute.For<IOrderService>();
            _securityUtil = Substitute.For<ISecurityUtil>();
            _refundService = Substitute.For<IRefundService>();
            _orderDetailsService = Substitute.For<IOrderDetailsService>();
            _controller = new OrderController(_orderService, _securityUtil, _orderDetailsService, _refundService);
        }


        [Fact]
        public async Task InitializeOrder_WithValidData_ReturnsOk()
        {
            //Arrange
            var form = new OrderForm
            {
                DeliveryAddressId = 1,
                ProductIds = new[] { 1, 2, 3 },
            };
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.BadRequest };
            _orderService.AddOrderAsync(_securityUtil.GetCurrentUserId(), form.DeliveryAddressId, form.ProductIds).Returns(expectedResult);

            // Act
            var result = await _controller.InitializeOrder(form) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);

        }

        [Fact]
        public async Task InitializeOrder_WithNoProducts_ReturnsBadRequest()
        {
            // Arrange
            var form = new OrderForm
            {
                DeliveryAddressId = 1,
                ProductIds = Array.Empty<int>()
            };

            // Act
            var result = await _controller.InitializeOrder(form) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);

            var errorMessage = result.Value as string;
            Assert.NotNull(errorMessage);
            Assert.Equal("No products found", errorMessage);
        }

        [Fact]
        public async Task InitializeOrder_ReturnsOk()
        {
            // Arrange
            var form = new OrderForm
            {
                DeliveryAddressId = 1,
                ProductIds = new[] { 1, 2, 3 },
            };

            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _orderService.AddOrderAsync(_securityUtil.GetCurrentUserId(), form.DeliveryAddressId, form.ProductIds).Returns(expectedResult);

            // Act
            var result = await _controller.InitializeOrder(form) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task InitializeOrder_UnauthorizedUser_ReturnsUnauthorized()
        {
            // Arrange
            var form = new OrderForm
            {
                DeliveryAddressId = 1,
                ProductIds = new[] { 1, 2, 3 },
            };
            int userId = 1;

            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Unauthorized };
            _orderService.AddOrderAsync(userId, form.DeliveryAddressId, form.ProductIds).Returns(expectedResult);
            _securityUtil.GetCurrentUserId().Returns(userId);

            // Act
            var result = await _controller.InitializeOrder(form) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task ConfirmPayment_ReturnsOk()
        {
            // Arrange
            var confirmPaymentForm = new ConfirmPaymentForm
            {
                RazorpayOrderId = "pay_65856",
                RazorpayPaymentId = "payment_ref_123",
                RazorpaySignature = "payment",

            };
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _orderService.ConfirmPayment(confirmPaymentForm, _securityUtil.GetCurrentUserId()).Returns(expectedResult);

            // Act
            var result = await _controller.ConfirmPayment(confirmPaymentForm) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.IsType<ServiceResult>(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);

        }
        [Fact]
        public async Task ConfirmPayment_BadRequestk()
        {
            // Arrange
            var confirmPaymentForm = new ConfirmPaymentForm
            {
                RazorpayOrderId = "pay_65856",
                RazorpayPaymentId = "payment_ref_123",
                RazorpaySignature = "payment",

            };
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.BadRequest };
            _orderService.ConfirmPayment(confirmPaymentForm, _securityUtil.GetCurrentUserId()).Returns(expectedResult);

            // Act
            var result = await _controller.ConfirmPayment(confirmPaymentForm) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.IsType<ServiceResult>(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);

        }

        [Fact]
        public async Task ConfirmPayment_unauthorized()
        {
            // Arrange
            var confirmPaymentForm = new ConfirmPaymentForm
            {
                RazorpayOrderId = "pay_65856",
                RazorpayPaymentId = "payment_ref_123",
                RazorpaySignature = "payment",

            };
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Unauthorized };
            _orderService.ConfirmPayment(confirmPaymentForm, _securityUtil.GetCurrentUserId()).Returns(expectedResult);

            // Act
            var result = await _controller.ConfirmPayment(confirmPaymentForm) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.IsType<ServiceResult>(result.Value);
            Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);

        }

        [Fact]
        public async Task ViewOrderDetailList_ReturnsOk()
        {
            // Arrange
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _orderDetailsService.GetAllOrders(_securityUtil.GetCurrentUserId()).Returns(expectedResult);

            // Act
            var result = await _controller.ViewOrderDetailList() as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task ViewOrderDetailList_Badrequest()
        {
            // Arrange
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.BadRequest };
            _orderDetailsService.GetAllOrders(_securityUtil.GetCurrentUserId()).Returns(expectedResult);

            // Act
            var result = await _controller.ViewOrderDetailList() as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task ViewOrderDetailList_unauthorized()
        {
            // Arrange
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Unauthorized };
            _orderDetailsService.GetAllOrders(_securityUtil.GetCurrentUserId()).Returns(expectedResult);

            // Act
            var result = await _controller.ViewOrderDetailList() as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task ViewOrderDetails_ReturnsOk()
        {
            // Arrange
            int orderDetailsId = 1;
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _orderDetailsService.GetOrderDetailsById(_securityUtil.GetCurrentUserId(), orderDetailsId).Returns(expectedResult);

            // Act
            var result = await _controller.ViewOrderDetails(orderDetailsId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task ViewOrderDetails_BadRequest()
        {
            // Arrange
            int orderDetailsId = 1;
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.BadRequest };
            _orderDetailsService.GetOrderDetailsById(_securityUtil.GetCurrentUserId(), orderDetailsId).Returns(expectedResult);

            // Act
            var result = await _controller.ViewOrderDetails(orderDetailsId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task ViewOrderDetails_unauthorized()
        {
            // Arrange
            int orderDetailsId = 1;
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Unauthorized };
            _orderDetailsService.GetOrderDetailsById(_securityUtil.GetCurrentUserId(), orderDetailsId).Returns(expectedResult);

            // Act
            var result = await _controller.ViewOrderDetails(orderDetailsId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task CancelOrder_ReturnsOk()
        {
            // Arrange
            var orderNumber = "12345";
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _orderService.CancelOrder(orderNumber).Returns(expectedResult);

            // Act
            var result = await _controller.CancelOrder(orderNumber) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task CancelOrder_Badrequest()
        {
            // Arrange
            var orderNumber = "12345";
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.BadRequest };
            _orderService.CancelOrder(orderNumber).Returns(expectedResult);

            // Act
            var result = await _controller.CancelOrder(orderNumber) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task CancelOrder_unauthorized()
        {
            // Arrange
            var orderNumber = "12345";
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Unauthorized };
            _orderService.CancelOrder(orderNumber).Returns(expectedResult);

            // Act
            var result = await _controller.CancelOrder(orderNumber) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task DownloadInvoice_ReturnsFile()
        {
            // Arrange
            int orderDetailsId = 123;
            var expectedResult = new MemoryStream();
            _orderService.DownloadInvoice(orderDetailsId, _securityUtil.GetCurrentUserId()).Returns(expectedResult);

            // Act
            var result = await _controller.DownloadInvoice(orderDetailsId) as FileContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("application/pdf", result.ContentType);
            Assert.Equal($"invoice_{orderDetailsId}.pdf", result.FileDownloadName);
            Assert.Equal(expectedResult.ToArray(), result.FileContents);
        }


        [Fact]
        public async Task EmailInvoice_ReturnsOk()
        {
            // Arrange
            var emailService = Substitute.For<IEmailService>();
            var orderDetailsId = 123;
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };

            _orderService.EmailInvoice(orderDetailsId, _securityUtil.GetCurrentUserId()).Returns(expectedResult);

            // Act
            var result = await _controller.EmailInvoice(orderDetailsId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }



        //[Fact]
        //public async Task RefundOrder_ReturnsExpectedResult()
        //{
        //    // Arrange
        //    var form = new RefundOrderForm { Reason = "Item not as described" };
        //    var orderId = 123;
        //    var productId = 456;
        //    var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
        //    _refundService.Refund(form, orderId, productId, _securityUtil.GetCurrentUserId()).Returns(expectedResult);

        //    // Act
        //    var result = await _controller.RefundOrder(form, orderId, productId) as ObjectResult;

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.NotNull(result.Value);
        //    Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        //    Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        //}

        //[Fact]
        //public async Task RefundOrder_Badrequest()
        //{
        //    // Arrange
        //    var form = new RefundOrderForm { Reason = "Item not as described" };
        //    var orderId = 123;
        //    var productId = 456;
        //    var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.BadRequest };
        //    _refundService.Refund(form, orderId, productId, _securityUtil.GetCurrentUserId()).Returns(expectedResult);

        //    // Act
        //    var result = await _controller.RefundOrder(form, orderId, productId) as ObjectResult;

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.NotNull(result.Value);
        //    Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        //    Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        //}

        //[Fact]
        //public async Task RefundOrder_unauthorized()
        //{
        //    // Arrange
        //    var form = new RefundOrderForm { Reason = "Item not as described" };
        //    var orderId = 123;
        //    var productId = 456;
        //    var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Unauthorized };
        //    _refundService.Refund(form, orderId, productId, _securityUtil.GetCurrentUserId()).Returns(expectedResult);

        //    // Act
        //    var result = await _controller.RefundOrder(form, orderId, productId) as ObjectResult;

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.NotNull(result.Value);
        //    Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
        //    Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        //}

        [Fact]
        public async Task DownloadInvoice()
        {
            // Arrange
            var mockOrderService = new Mock<IOrderService>();
            var mockSecurityUtil = new Mock<ISecurityUtil>();
            var orderdetails = new Mock<IOrderDetailsService>();
            var refund = new Mock<IRefundService>();
            var controller = new OrderController(mockOrderService.Object, mockSecurityUtil.Object, orderdetails.Object, refund.Object);

            var orderDetailsId = 123;
            var userId = 456;
            var serviceResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };

            mockSecurityUtil.Setup(s => s.GetCurrentUserId()).Returns(userId);
            mockOrderService.Setup(s => s.DownloadInvoice(orderDetailsId, userId)).ReturnsAsync(serviceResult);

            // Act
            var result = await controller.DownloadInvoice(orderDetailsId);

            // Assert
            Assert.IsType<ObjectResult>(result);

            var objectResult = (ObjectResult)result;
            Assert.Equal((int)serviceResult.ServiceStatus, objectResult.StatusCode);
            Assert.Equal(serviceResult, objectResult.Value);
            mockOrderService.Verify(s => s.DownloadInvoice(orderDetailsId, userId), Times.Once);

        }

        [Fact]
        public async Task DownloadInvoice_Returns_BadRequest_If_Service_Result_Is_Not_A_MemoryStream()
        {
            // Arrange
            var _orderServiceMock = new Mock<IOrderService>();
            var _securityUtilMock = new Mock<ISecurityUtil>();
            var orderdetails = new Mock<IOrderDetailsService>();
            var refund = new Mock<IRefundService>();
            var _controller = new OrderController(_orderServiceMock.Object, _securityUtilMock.Object, orderdetails.Object, refund.Object);
            int orderDetailsId = 123;
            _orderServiceMock.Setup(x => x.DownloadInvoice(orderDetailsId, It.IsAny<int>()))
                             .ReturnsAsync(new ServiceResult { ServiceStatus = ServiceStatus.NotFound });

            // Act
            var result = await _controller.DownloadInvoice(orderDetailsId);

            // Assert
            var badRequestResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.NotFound, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task DownloadInvoice_Returns_File_If_Service_Result_Is_A_MemoryStream()
        {
            // Arrange
            var _orderServiceMock = new Mock<IOrderService>();
            var _securityUtilMock = new Mock<ISecurityUtil>();
            var orderdetails = new Mock<IOrderDetailsService>();
            var refund = new Mock<IRefundService>();
            var _controller = new OrderController(_orderServiceMock.Object, _securityUtilMock.Object, orderdetails.Object, refund.Object);
            int orderDetailsId = 123;
            var memoryStream = new MemoryStream(new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2d, 0x31, 0x2e, 0x35 });
            _orderServiceMock.Setup(x => x.DownloadInvoice(orderDetailsId, It.IsAny<int>()))
                             .ReturnsAsync(memoryStream);

            // Act
            var result = await _controller.DownloadInvoice(orderDetailsId);

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("application/pdf", fileResult.ContentType);
            Assert.Equal($"invoice_{orderDetailsId}.pdf", fileResult.FileDownloadName);
            Assert.Equal(memoryStream.ToArray(), fileResult.FileContents);
        }

        [Fact]
        public async Task DownloadInvoice_Returns_Status_Code_From_Service_Result()
        {
            // Arrange
            var _orderServiceMock = new Mock<IOrderService>();
            var _securityUtilMock = new Mock<ISecurityUtil>();
            var orderdetails = new Mock<IOrderDetailsService>();
            var refund = new Mock<IRefundService>();
            var _controller = new OrderController(_orderServiceMock.Object, _securityUtilMock.Object, orderdetails.Object, refund.Object);
            int orderDetailsId = 123;
            _orderServiceMock.Setup(x => x.DownloadInvoice(orderDetailsId, It.IsAny<int>()))
                             .ReturnsAsync(new ServiceResult { ServiceStatus = ServiceStatus.Unauthorized });

            // Act
            var result = await _controller.DownloadInvoice(orderDetailsId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)ServiceStatus.Unauthorized, statusCodeResult.StatusCode);
            Assert.IsType<ServiceResult>(statusCodeResult.Value);
        }

        [Fact]
        public async Task DownloadInvoice_ValidOrderDetailsId_ReturnsFileResult()
        {
            // Arrange
            int orderDetailsId = 123;
            var mockMemoryStream = new MemoryStream(new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D, 0x31, 0x2E, 0x35 }); // %PDF-1.5
            var _orderServiceMock = new Mock<IOrderService>();
            var _securityUtilMock = new Mock<ISecurityUtil>();
            var orderdetails = new Mock<IOrderDetailsService>();
            var refund = new Mock<IRefundService>();
            var _controller = new OrderController(_orderServiceMock.Object, _securityUtilMock.Object, orderdetails.Object, refund.Object);
            _orderServiceMock.Setup(s => s.DownloadInvoice(orderDetailsId, It.IsAny<int>())).ReturnsAsync(mockMemoryStream);

            // Act
            var result = await _controller.DownloadInvoice(orderDetailsId);

            // Assert
            Assert.IsType<FileContentResult>(result);
            var fileResult = result as FileContentResult;
            Assert.Equal("application/pdf", fileResult.ContentType);
            Assert.Equal($"invoice_{orderDetailsId}.pdf", fileResult.FileDownloadName);
            Assert.Equal(mockMemoryStream.ToArray(), fileResult.FileContents);
        }


        [Fact]
        public async Task DownloadInvoice_NullServiceResult_ReturnsBadRequestResult()
        {
            // Arrange
            int orderDetailsId = 123;
            MemoryStream nullMemoryStream = null;
            var _orderServiceMock = new Mock<IOrderService>();
            var _securityUtilMock = new Mock<ISecurityUtil>();
            var orderdetails = new Mock<IOrderDetailsService>();
            var refund = new Mock<IRefundService>();
            var _controller = new OrderController(_orderServiceMock.Object, _securityUtilMock.Object, orderdetails.Object, refund.Object);
            _orderServiceMock.Setup(s => s.DownloadInvoice(orderDetailsId, It.IsAny<int>())).ReturnsAsync(nullMemoryStream);

            // Act
            var result = await _controller.DownloadInvoice(orderDetailsId);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task GetHistoryTest()
        {
            //Arrange
            int userId = 2;
            int orderDetailsId = 10;

            _securityUtil.GetCurrentUserId().Returns(userId);

            _orderService.GetOrderHistory(orderDetailsId, userId).Returns(new ServiceResult() { ServiceStatus = ServiceStatus.Success });

            ServiceResult expectedResult = new() { ServiceStatus = ServiceStatus.Success };

            //Act
            var actualResult = await _controller.GetOrderHistory(10) as ObjectResult;


            //Assert
            Assert.NotNull(actualResult);
            Assert.NotNull(actualResult.Value);
            Assert.Equal(actualResult.StatusCode, StatusCodes.Status200OK);

        }
    }

}