using MarketPlace.DataAccess.Interfaces;
using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Dto.Views;
using MarketPlaceAdmin.Bussiness.Enums;
using MarketPlaceAdmin.Bussiness.Services;
using NSubstitute;
using System.Globalization;
using Xunit;

namespace MarketplaceAdminTest.Service
{
    public class DashboardServiceTests
    {
        [Fact]
        public async Task GetActiveProductCountGroupByCategory_ReturnsCorrectCount()
        {
            // Arrange
            var mockUnitOfWork = Substitute.For<IUnitOfWork>();

            var dashboardService = new DashboardService(mockUnitOfWork);


            var expectedCategoryProductCount = new Dictionary<string, int>()
                {
                    { "Category A", 10 },
                    { "Category B", 5 },
                    { "Category C", 3 }
                };

            mockUnitOfWork.ProductRepostory.GetActiveProductCountGroupByCategory().Returns(expectedCategoryProductCount);


            // Act
            var result = await dashboardService.GetActiveProductCountGroupByCategory();

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Category Wise Product Count", result.Message);

            Assert.NotNull(result.Data);
            Assert.Equal(expectedCategoryProductCount.Count, ((IEnumerable<CountView>)result.Data).Count());

            foreach (var category in expectedCategoryProductCount.Keys)
            {
                var expectedCount = expectedCategoryProductCount[category];
                var actualCount = ((IEnumerable<CountView>?)result.Data)?.Single(x => x.Property == category).Count;

                Assert.Equal(expectedCount, actualCount);
            }
        }

        [Fact]
        public async Task GetSalesCount_WithValidFromToDateForm_ReturnsSuccessResult()
        {
            // Arrange
            var form = new FromToDateForm { From = "2022-01-01", To = "2022-01-04" };

            var unitOfWorkMock = Substitute.For<IUnitOfWork>();
            var expectedSalesCount = new Dictionary<DateTime, int>
            {
                { new DateTime(2022, 01, 01), 10 },
                { new DateTime(2022, 01, 02), 5 },
                { new DateTime(2022, 01, 03), 3 }
            };
            DateTime.TryParseExact(form.From, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime from);
            DateTime.TryParseExact(form.To, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime to);

            unitOfWorkMock.OrderDetailsRepository.GetSalesCount(from, to).Returns(expectedSalesCount);

            var dashboardService = new DashboardService(unitOfWorkMock);

            // Act
            var result = await dashboardService.GetSalesCount(form);

            // Assert

            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Sales Count", result.Message);
            Assert.NotNull(result.Data);
            Assert.Equal(4, ((IEnumerable<CountView>)result.Data).Count());

            var salesCounts = ((IEnumerable<CountView>?)result.Data)?.Cast<CountView>().ToDictionary(cv => DateTime.Parse(cv.Property), cv => cv.Count);
            Assert.NotNull(salesCounts);
            Assert.Equal(10, salesCounts[new DateTime(2022, 01, 01)]);
            Assert.Equal(5, salesCounts[new DateTime(2022, 01, 02)]);
            Assert.Equal(3, salesCounts[new DateTime(2022, 01, 03)]);
        }

        [Fact]
        public async Task GetSalesCount_WithInvalidFromToDateForm_ReturnsBadRequestResult()
        {
            // Arrange
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var dashboardService = new DashboardService(unitOfWork);
            var form = new FromToDateForm { From = "2022-01-01", To = "2022-01-10-Invalid" };
            var form2 = new FromToDateForm { From = "2022-01-01-invalid", To = "2022-01-10" };

            // Act
            var result = await dashboardService.GetSalesCount(form);
            var result2 = await dashboardService.GetSalesCount(form2);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Date", result.Message);

            Assert.Equal(ServiceStatus.BadRequest, result2.ServiceStatus);
            Assert.Equal("Invalid Date", result2.Message);
        }

        [Fact]
        public async Task GetSalesCount_WithToLessThanFromDate_ReturnsBadRequestResult()
        {
            // Arrange
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var dashboardService = new DashboardService(unitOfWork);
            var form = new FromToDateForm { From = "2022-01-10", To = "2022-01-01" };

            // Act
            var result = await dashboardService.GetSalesCount(form);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("From Date should be less than To date", result.Message);
        }
    }
}
