using System.ComponentModel.DataAnnotations;

namespace MarketPlaceAdmin.Bussiness.Dto.Forms
{
    /// <summary>
    /// Represents a form used to create or update a category.
    /// </summary>
    public class CategoryForm
    {
        /// <summary>
        /// Gets or sets the name of the category.
        /// </summary>
        /// <remarks>
        /// This property is required and must not be empty.
        /// The category name must be a string of alphabets and spaces only.
        /// </remarks>
        [Required(ErrorMessage = "Category name is required", AllowEmptyStrings = false)]
        [StringLength(20)]
        [RegularExpression("^[A-Za-z\\s]*$", ErrorMessage = "Only Alphabets and space allowed.")]
        public string CategoryName { get; set; } = null!;
    }
}