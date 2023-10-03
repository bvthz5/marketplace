using System.ComponentModel.DataAnnotations;

namespace MarketPlaceAdmin.Bussiness.Dto.Forms
{
    /// <summary>
    /// Represents the parameters used for paginating and filtering a list of products.
    /// </summary>
    public class ProductPaginationParams : PaginationSearchSortingParams
    {
        /// <summary>
        /// An array of category IDs used to filter the products.
        /// </summary>
        /// <value>The array of category IDs.</value>
        public int?[]? CategoryId { get; set; }

        /// <summary>
        /// The minimum price for the products.
        /// </summary>
        /// <value>The minimum price.</value>
        [Range(0.00, 10000000.00)]
        public float StartPrice { get; set; } = 0;

        /// <summary>
        /// The maximum price for the products.
        /// </summary>
        /// <value>The maximum price.</value>
        [Range(0.00, 10000000.00)]
        public float EndPrice { get; set; } = 0;

        /// <summary>
        /// The location used to filter the products.
        /// </summary>
        /// <value>The location value.</value>
        [StringLength(200)]
        public string? Location { get; set; }

        /// <summary>
        /// A list of statuses used to filter the products.
        /// </summary>
        /// <value>The list of statuses.</value>
        public List<byte?>? Status { get; set; }

        /// <summary>
        /// The ID of the user whose products to filter.
        /// </summary>
        /// <value>The user ID.</value>
        public int? UserId { get; set; } = null;
    }
}
