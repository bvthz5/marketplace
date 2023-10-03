using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlaceAdmin.Api.Controllers
{
    /// <summary>
    /// API controller for managing categories.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/category")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        /// <summary>
        /// Creates a new instance of the CategoryController class.
        /// </summary>
        /// <param name="categoryService">The service used to manage categories.</param>
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Adds a new category to the system.
        /// </summary>
        /// <param name="form">The category information to be added.</param>
        /// <returns>A ServiceResult object indicating the success or failure of the operation.</returns>
        [HttpPost(Name = "Add Category")]
        public async Task<IActionResult> Add(CategoryForm form)
        {
            ServiceResult result = await _categoryService.AddCategory(form);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Retrieves a list of all categories.
        /// </summary>
        /// <returns>A ServiceResult object containing the list of categories.</returns>
        [HttpGet(Name = "Category List")]
        public async Task<IActionResult> GetCategoryList()
        {
            ServiceResult result = await _categoryService.GetCategoryList();
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Updates an existing category in the system.
        /// </summary>
        /// <param name="categoryId">The ID of the category to be updated.</param>
        /// <param name="form">The new category information.</param>
        /// <returns>A ServiceResult object indicating the success or failure of the operation.</returns>
        [HttpPut("{categoryId:int:min(1)}", Name = "Edit Category")]
        public async Task<IActionResult> Update(int categoryId, [FromBody] CategoryForm form)
        {
            ServiceResult result = await _categoryService.EditCategory(form, categoryId);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Changes the status of an existing category to active or inactive.
        /// </summary>
        /// <param name="categoryId">The ID of the category to be updated.</param>
        /// <param name="status">The new status value (0 for inactive, 1 for active).</param>
        /// <returns>A ServiceResult object indicating the success or failure of the operation.</returns>
        [HttpPut("status/{categoryId:int:min(1)}", Name = "Change Category Status")]
        public async Task<IActionResult> ChangeStatus(int categoryId, [FromBody] byte status)
        {
            ServiceResult result = await _categoryService.ChangeCategoryStatus(categoryId, status);
            return StatusCode((int)result.ServiceStatus, result);
        }
    }
}
