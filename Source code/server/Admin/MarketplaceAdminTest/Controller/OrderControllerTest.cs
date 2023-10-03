using MarketPlaceAdmin.Api.Controllers;
using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Enums;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace MarketplaceAdminTest.Controller
{
    public class OrderControllerTest
    {

        [Fact]
        public async Task OrderDetailsSuccessTest()
        {
            //Arrange

            IAdminOrderService orderService = Substitute.For<IAdminOrderService>();

            orderService.GetOrderDetails(10).Returns(new ServiceResult() { ServiceStatus = ServiceStatus.Success });

            OrderController orderController = new(orderService);

            ServiceResult expectedResult = new() { ServiceStatus = ServiceStatus.Success };

            //Act

            var actualResult = await orderController.GetOrderDetails(10) as ObjectResult;

            //Assert
            Assert.NotNull(actualResult);
            Assert.NotNull(actualResult.Value);
            Assert.Equal(actualResult.StatusCode, StatusCodes.Status200OK);
            Assert.Equal(((ServiceResult)actualResult.Value).ServiceStatus, expectedResult.ServiceStatus);
        }

        [Fact]
        public async Task RefreshSuccessTest()
        {
            //Arrange

            IAdminOrderService orderService = Substitute.For<IAdminOrderService>();

            OrderPaginationParams orderPaginationParams = new();

            orderService.GetOrderList(orderPaginationParams).Returns(new ServiceResult() { ServiceStatus = ServiceStatus.Success });

            OrderController orderController = new(orderService);

            ServiceResult expectedResult = new() { ServiceStatus = ServiceStatus.Success };

            //Act

            var actualResult = await orderController.GetOrderList(orderPaginationParams) as ObjectResult;

            //Assert
            Assert.NotNull(actualResult);
            Assert.NotNull(actualResult.Value);
            Assert.Equal(actualResult.StatusCode, StatusCodes.Status200OK);
            Assert.Equal(((ServiceResult)actualResult.Value).ServiceStatus, expectedResult.ServiceStatus);
        }


        [Fact]

        public async Task GetHistoryTest()
        {
            //Arrange
            IAdminOrderService orderService = Substitute.For<IAdminOrderService>();

            orderService.GetOrderHistory(10).Returns(new ServiceResult() { ServiceStatus = ServiceStatus.Success });

            OrderController orderController = new(orderService);

            ServiceResult expectedResult = new() { ServiceStatus = ServiceStatus.Success };
            //Act
            var actualResult = await orderController.GetOrderHistory(10) as ObjectResult;
            //Assert
            Assert.NotNull(actualResult);
            Assert.NotNull(actualResult.Value);
            Assert.Equal(actualResult.StatusCode, StatusCodes.Status200OK);
            Assert.Equal(((ServiceResult)actualResult.Value).ServiceStatus, expectedResult.ServiceStatus);
        }
    }
}