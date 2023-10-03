using MarketPlaceAdmin.Api.Controllers;
using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Enums;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using MarketPlaceAdmin.Bussiness.Security.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace MarketplaceAdminTest.Controller
{
    public class AgentOrderControllerTests
    {
        private readonly IAgentOrderService _orderService;
        private readonly ISecurityUtil _securityUtil;

        private readonly AgentOrderController _controller;


        public AgentOrderControllerTests()
        {
            _orderService = Substitute.For<IAgentOrderService>();
            _securityUtil = Substitute.For<ISecurityUtil>();

            _controller = new(_orderService, _securityUtil);
        }

        [Theory]
        [InlineData(200)]
        [InlineData(400)]
        [InlineData(404)]
        public async Task GetOrderList(int statusCode)
        {
            // Arrange

            AgentOrderPaginationParams form = new();

            _orderService.GetOrderList(form).Returns(new ServiceResult() { ServiceStatus = (ServiceStatus)statusCode });

            // Act 

            var result = await _controller.GetOrderList(form) as ObjectResult;

            // Assert   

            Assert.NotNull(result);
            Assert.Equal(statusCode, result.StatusCode);

            Assert.IsType<ServiceResult>(result.Value);

            Assert.Equal((ServiceStatus)statusCode, ((ServiceResult)result.Value).ServiceStatus);

        }


        [Theory]
        [InlineData(200)]
        [InlineData(400)]
        [InlineData(404)]
        public async Task AssignOrder(int statusCode)
        {
            // Arrange

            int orderId = 10;

            _orderService.AssignOrder(orderId).Returns(new ServiceResult() { ServiceStatus = (ServiceStatus)statusCode });

            // Act 

            var result = await _controller.AssignOrder(orderId) as ObjectResult;

            // Assert   

            Assert.NotNull(result);
            Assert.Equal(statusCode, result.StatusCode);

            Assert.IsType<ServiceResult>(result.Value);

            Assert.Equal((ServiceStatus)statusCode, ((ServiceResult)result.Value).ServiceStatus);

        }

        [Theory]
        [InlineData(200)]
        [InlineData(400)]
        [InlineData(404)]
        public async Task UnAssignOrder(int statusCode)
        {
            // Arrange

            int orderId = 10;

            _orderService.UnAssignOrder(orderId).Returns(new ServiceResult() { ServiceStatus = (ServiceStatus)statusCode });

            // Act 

            var result = await _controller.UnAssignOrder(orderId) as ObjectResult;

            // Assert   

            Assert.NotNull(result);
            Assert.Equal(statusCode, result.StatusCode);

            Assert.IsType<ServiceResult>(result.Value);

            Assert.Equal((ServiceStatus)statusCode, ((ServiceResult)result.Value).ServiceStatus);

        }


        [Theory]
        [InlineData(200)]
        [InlineData(400)]
        [InlineData(404)]
        public async Task AgentOrderDetailView(int statusCode)
        {
            // Arrange

            int orderId = 10;
            int agentId = 1;

            _securityUtil.GetCurrentUserId().Returns(agentId);

            _orderService.GetOrderDetails(orderId, agentId).Returns(new ServiceResult() { ServiceStatus = (ServiceStatus)statusCode });

            // Act 

            var result = await _controller.AgentOrderDetailView(orderId) as ObjectResult;

            // Assert   

            Assert.NotNull(result);
            Assert.Equal(statusCode, result.StatusCode);

            Assert.IsType<ServiceResult>(result.Value);

            Assert.Equal((ServiceStatus)statusCode, ((ServiceResult)result.Value).ServiceStatus);

        }

        [Theory]
        [InlineData(200)]
        [InlineData(400)]
        [InlineData(404)]
        public async Task ChangeDeliveryStatus(int statusCode)
        {
            // Arrange

            int orderId = 10;
            int agentId = 1;
            byte status = 2;

            _securityUtil.GetCurrentUserId().Returns(agentId);

            _orderService.ChangeDeliveryStatus(orderId, status, agentId).Returns(new ServiceResult() { ServiceStatus = (ServiceStatus)statusCode });

            // Act 

            var result = await _controller.ChangeDeliveryStatus(orderId, status) as ObjectResult;

            // Assert   

            Assert.NotNull(result);
            Assert.Equal(statusCode, result.StatusCode);

            Assert.IsType<ServiceResult>(result.Value);

            Assert.Equal((ServiceStatus)statusCode, ((ServiceResult)result.Value).ServiceStatus);

        }

        [Theory]
        [InlineData(200)]
        [InlineData(400)]
        [InlineData(404)]
        public async Task AgentOrderStatusCount(int statusCode)
        {
            //Arrange

            int agentId = 10;
            _securityUtil.GetCurrentUserId().Returns(agentId);
            _orderService.AgentOrdersStatusCount(agentId).Returns(new ServiceResult() { ServiceStatus = (ServiceStatus)statusCode });


            //Act

            var result = await _controller.AgentOrdersStatusCount() as ObjectResult;

            //Assert

            Assert.NotNull(result);
            Assert.Equal(statusCode, result.StatusCode);
            Assert.IsType<ServiceResult>(result.Value);
            Assert.Equal((ServiceStatus)statusCode, ((ServiceResult)result.Value).ServiceStatus);
        }


        [Theory]
        [InlineData(200)]
        [InlineData(400)]
        [InlineData(404)]
        public async Task GenerateOtp(int statusCode)
        {
            //Arrange

            int orderId = 10;
            int agentId = 10;
            _securityUtil.GetCurrentUserId().Returns(agentId);
            _orderService.GenerateOtp(orderId, agentId).Returns(new ServiceResult() { ServiceStatus = (ServiceStatus)statusCode });

            //Act

            var result = await _controller.GenerateOtp(orderId) as ObjectResult;

            //Assert

            Assert.NotNull(result);
            Assert.Equal(statusCode, result.StatusCode);
            Assert.IsType<ServiceResult>(result.Value);
            Assert.Equal((ServiceStatus)statusCode, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Theory]
        [InlineData(200)]

        public async Task VerifyOtp(int statusCode)
        {
            //Arrange

            int orderId = 10;
            int agentId = 10;
            string otp = "444444";
            _securityUtil.GetCurrentUserId().Returns(agentId);
            _orderService.VerifyOtp(orderId, agentId, otp).Returns(new ServiceResult() { ServiceStatus = (ServiceStatus)statusCode });

            //Act

            var result = await _controller.VerifyOtp(orderId, otp) as ObjectResult;

            //Assert

            Assert.NotNull(result);
            Assert.Equal(statusCode, result.StatusCode);
            Assert.IsType<ServiceResult>(result.Value);
            Assert.Equal((ServiceStatus)statusCode, ((ServiceResult)result.Value).ServiceStatus);
        }
    }
}
