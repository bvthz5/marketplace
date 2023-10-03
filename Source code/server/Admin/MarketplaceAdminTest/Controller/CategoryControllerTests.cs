using MarketPlace.DataAccess.Model;
using MarketPlaceAdmin.Api.Controllers;
using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Enums;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace MarketplaceAdminTest.Controller
{
    public class CategoryControllerTests
    {
        private readonly ICategoryService _categoryService;
        private readonly CategoryController _controller;

        public CategoryControllerTests()
        {
            _categoryService = Substitute.For<ICategoryService>();
            _controller = new CategoryController(_categoryService);
        }

        [Fact]
        public async Task Add_Category_Returns_Successful_Result()
        {
            // Arrange
            var categoryForm = new CategoryForm { CategoryName = "Test Category" };
            var serviceResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _categoryService.AddCategory(categoryForm).Returns(serviceResult);

            // Act
            var result = await _controller.Add(categoryForm);

            // Assert
            Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, ((ObjectResult)result).StatusCode);
            Assert.Equal(serviceResult, ((ObjectResult)result).Value);
        }

        [Fact]
        public async Task GetCategoryList_ReturnsSuccessResult_WhenServiceReturnsSuccess()
        {
            // Arrange
            var result = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            var categoryList = new List<Category> { new Category { CategoryId = 1, CategoryName = "Test Category" } };
            result.Data = categoryList;
            _categoryService.GetCategoryList().Returns(result);

            // Act
            var response = await _controller.GetCategoryList();

            // Assert
            Assert.IsType<ObjectResult>(response);
            var objectResult = (ObjectResult)response;
            Assert.Equal((int)result.ServiceStatus, objectResult.StatusCode);
            Assert.Equal(result, objectResult.Value);
            Assert.Equal(categoryList, result.Data);
        }

        [Fact]
        public async Task Update_ReturnsSuccessResult_WhenServiceReturnsSuccess()
        {
            // Arrange
            var categoryId = 1;
            var form = new CategoryForm { CategoryName = "Updated Category" };
            var result = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _categoryService.EditCategory(form, categoryId).Returns(result);

            // Act
            var response = await _controller.Update(categoryId, form);

            // Assert
            Assert.IsType<ObjectResult>(response);
            var objectResult = (ObjectResult)response;
            Assert.Equal((int)result.ServiceStatus, objectResult.StatusCode);
            Assert.Equal(result, objectResult.Value);
        }

        [Fact]
        public async Task ChangeStatus_ReturnsSuccessResult_WhenServiceReturnsSuccess()
        {
            // Arrange
            var categoryId = 1;
            byte status = 1;
            var result = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _categoryService.ChangeCategoryStatus(categoryId, status).Returns(result);

            // Act
            var response = await _controller.ChangeStatus(categoryId, status);

            // Assert
            Assert.IsType<ObjectResult>(response);
            var objectResult = (ObjectResult)response;
            Assert.Equal((int)result.ServiceStatus, objectResult.StatusCode);
            Assert.Equal(result, objectResult.Value);
        }
    }
}
