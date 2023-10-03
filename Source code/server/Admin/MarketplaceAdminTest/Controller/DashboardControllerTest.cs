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
    public class DashboardControllerTest
    {
        [Fact]
        public async Task DashboardGetCategoryProductCountSuccess()
        {
            // Arrange
            IDashboardService dashboardService = Substitute.For<IDashboardService>();

            dashboardService.GetActiveProductCountGroupByCategory().Returns(new ServiceResult() { ServiceStatus = ServiceStatus.Success, Data = null, Message = "Category Wise Product Count" });

            ServiceResult expectedResult = new() { ServiceStatus = ServiceStatus.Success };

            DashboardController dashboardController = new(dashboardService);

            // Act
            var actualResult = await dashboardController.GetCategoryProductCount() as ObjectResult;

            //Assert
            Assert.NotNull(actualResult);
            Assert.NotNull(actualResult.Value);
            Assert.IsAssignableFrom<ServiceResult>(actualResult.Value);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)actualResult.Value).ServiceStatus);
            Assert.True(((ServiceResult)actualResult.Value).Status);
            Assert.Equal(StatusCodes.Status200OK, actualResult.StatusCode);
        }

        [Fact]
        public void DashboardGetCategoryProductCountException()
        {
            // Arrange
            IDashboardService dashboardService = Substitute.For<IDashboardService>();

            dashboardService.When(x => x.GetActiveProductCountGroupByCategory())
                .Do(x => throw new Exception());

            DashboardController dashboardController = new(dashboardService);

            // Act
            var actualResult = dashboardController.GetCategoryProductCount();

            //Assert
            Assert.ThrowsAsync<Exception>(() => actualResult);
        }

        [Fact]
        public async Task DashboardGetSalesCountSuccess()
        {
            // Arrange
            IDashboardService dashboardService = Substitute.For<IDashboardService>();

            FromToDateForm form = new()
            {
                From = "2022-10-12",
                To = "2022-10-15"
            };

            dashboardService.GetSalesCount(form).Returns(new ServiceResult() { ServiceStatus = ServiceStatus.Success });

            ServiceResult expectedResult = new() { ServiceStatus = ServiceStatus.Success };

            DashboardController dashboardController = new(dashboardService);

            // Act
            var actualResult = await dashboardController.GetOrderCount(form) as ObjectResult;

            //Assert
            Assert.NotNull(actualResult);
            Assert.NotNull(actualResult.Value);
            Assert.IsAssignableFrom<ServiceResult>(actualResult.Value);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)actualResult.Value).ServiceStatus);
            Assert.Equal(StatusCodes.Status200OK, actualResult.StatusCode);
        }

        [Fact]
        public void DashboardGetSalesCountException()
        {
            // Arrange
            FromToDateForm form = new()
            {
                From = "2022-10-12",
                To = "2022-10-15"
            };

            IDashboardService dashboardService = Substitute.For<IDashboardService>();

            dashboardService.When(x => x.GetSalesCount(form))
                .Do(x => throw new Exception());

            DashboardController dashboardController = new(dashboardService);

            // Act
            var actualResult = dashboardController.GetOrderCount(form);

            //Assert
            Assert.ThrowsAsync<Exception>(() => actualResult);
        }
    }
}