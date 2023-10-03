using MarketPlaceUser.Api.Controllers;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace MarketPlaceUserTest.TestController
{
    public class CategoryTestController
    {
        [Fact]
        public async Task CategoryListTest()
        {
            //Arrange

            ICategoryService categoryService = Substitute.For<ICategoryService>();

            categoryService.GetActiveCategoryList().Returns(new ServiceResult() { ServiceStatus = ServiceStatus.Success });

            CategoryController categoryController = new(categoryService);

            ServiceResult expectedResult = new() { ServiceStatus = ServiceStatus.Success };

            //Act

            var actualResult = await categoryController.GetCategoryActiveList() as ObjectResult;

            //Assert

            Assert.Equal(actualResult?.StatusCode, StatusCodes.Status200OK);
            Assert.Equal((actualResult?.Value as ServiceResult)?.ServiceStatus, expectedResult.ServiceStatus);

        }
    }
}
