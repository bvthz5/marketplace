using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Dto.Views;
using MarketPlaceAdmin.Bussiness.Enums;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;

namespace MarketPlaceAdmin.Bussiness.Services
{
    /// <summary>
    /// Service class for managing categories in the system.
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _uow;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryService"/> class.
        /// </summary>
        /// <param name="uow">The unit of work instance to use for data access.</param>
        public CategoryService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        /// <summary>
        /// Adds a new category to the database using information from the provided <paramref name="form"/>.
        /// </summary>
        /// <param name="form">The form containing information about the category to add.</param>
        /// <returns>
        /// A <see cref="ServiceResult"/> object indicating the result of the operation. 
        /// If the category already exists, the returned object will have a <see cref="ServiceStatus.AlreadyExists"/> status code and an appropriate message. 
        /// Otherwise, the object will have a <see cref="ServiceStatus.Success"/> status code and a message indicating that the category was added successfully.
        /// The <see cref="ServiceResult.Data"/> property of the returned object will contain a <see cref="CategoryDetailView"/> object representing the newly added category.
        /// </returns>
        public async Task<ServiceResult> AddCategory(CategoryForm form)
        {
            ServiceResult result = new();

            if (await _uow.CategoryRepository.FindByCategoryNameAsync(form.CategoryName.Trim()) != null)
            {
                result.ServiceStatus = ServiceStatus.AlreadyExists;
                result.Message = "Category Already Exists";
                return result;
            }

            var category = await _uow.CategoryRepository.Add(new Category()
            {
                CategoryName = form.CategoryName.Trim(),
                Status = Category.CategoryStatus.ACTIVE,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
            });

            await _uow.SaveAsync();

            result.ServiceStatus = ServiceStatus.Success;
            result.Message = "Category Added";
            result.Data = new CategoryDetailView(category);
            return result;
        }


        /// <summary>
        /// Edit Category using Category Name And Status form CategoryUpdateForm
        /// </summary>
        /// <param name="form">The form containing the updated category information.</param>
        /// <param name="categoryId">The ID of the category to be updated.</param>
        /// <returns>A ServiceResult object with CategoryDetailView data and error codes.</returns>
        public async Task<ServiceResult> EditCategory(CategoryForm form, int categoryId)
        {
            ServiceResult result = new();

            // Check if the category name already exists
            Category? category = await _uow.CategoryRepository.FindByCategoryNameAsync(form.CategoryName.Trim());
            if (category != null && category.CategoryId != categoryId)
            {
                result.ServiceStatus = ServiceStatus.AlreadyExists;
                result.Message = "Category Already Exists";
                return result;
            }

            // Check if the category exists and is active
            category = await _uow.CategoryRepository.FindByIdAndStatusAsync(categoryId, Category.CategoryStatus.ACTIVE);
            if (category is null)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Category Not Found";
                return result;
            }
            if (category.Status != Category.CategoryStatus.ACTIVE)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Inactive Category";
                return result;
            }

            // Update the category information and save changes to the database
            category.CategoryName = form.CategoryName.Trim();
            category.UpdatedDate = DateTime.Now;
            _uow.CategoryRepository.Update(category);
            await _uow.SaveAsync();

            result.ServiceStatus = ServiceStatus.Success;
            result.Message = "Category Name Updated";
            result.Data = new CategoryDetailView(category);

            return result;
        }

        /// <summary>
        /// Retrieves a list of all categories.
        /// </summary>
        /// <returns>A <see cref="ServiceResult"/> object that encapsulates the result of the operation, containing a list of <see cref="CategoryDetailView"/> objects on success.</returns>
        public async Task<ServiceResult> GetCategoryList()
        {
            // Call the FindAllAsync method to retrieve all categories from the repository.
            List<Category> categories = await _uow.CategoryRepository.FindAllAsync();

            // Convert each Category object to a CategoryDetailView object and add it to a list.
            List<CategoryDetailView> categoryDetails = categories.ConvertAll(category => new CategoryDetailView(category));

            // Return a ServiceResult object containing the list of CategoryDetailView objects.
            return new ServiceResult()
            {
                ServiceStatus = ServiceStatus.Success,
                Message = "Category List",
                Data = categoryDetails,
            };
        }

        /// <summary>
        /// Change the status of a category to either Active or Inactive.
        /// </summary>
        /// <param name="categoryId">The ID of the category to change status for.</param>
        /// <param name="status">The new status to set for the category. Valid values are either Category.CategoryStatus.ACTIVE or Category.CategoryStatus.INACTIVE.</param>
        /// <returns>A ServiceResult object containing the updated category details if successful. If the category is not found or the status provided is invalid, an error message will be returned in the ServiceResult.</returns>
        public async Task<ServiceResult> ChangeCategoryStatus(int categoryId, byte status)
        {
            ServiceResult result = new();

            Category? category = await _uow.CategoryRepository.FindById(categoryId);

            if (category is null)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Category Not Found";
                return result;
            }

            // Check if status is valid
            if (!Enum.IsDefined(typeof(Category.CategoryStatus), status))
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Invalid Status";
                return result;
            }

            category.Status = (Category.CategoryStatus)status;
            category.UpdatedDate = DateTime.Now;
            _uow.CategoryRepository.Update(category);

            await _uow.SaveAsync();

            result.ServiceStatus = ServiceStatus.Success;
            result.Message = "Status Changed";
            result.Data = new CategoryDetailView(category);
            return result;
        }
    }
}