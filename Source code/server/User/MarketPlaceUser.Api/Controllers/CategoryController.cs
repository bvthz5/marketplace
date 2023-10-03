using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlaceUser.Api.Controllers
{
    [ApiController]
    [Route("api/category")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        /// <summary>
        /// Gets a list of active categories.
        /// </summary>
        /// <returns>A <see cref="ServiceResult"/> containing the list of active categories.</returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetCategoryActiveList()
        {
            ServiceResult result = await _categoryService.GetActiveCategoryList();
            return StatusCode((int)result.ServiceStatus, result);
        }

    }
}
