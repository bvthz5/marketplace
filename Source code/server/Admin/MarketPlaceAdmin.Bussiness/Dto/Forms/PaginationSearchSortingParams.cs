using System.ComponentModel.DataAnnotations;

namespace MarketPlaceAdmin.Bussiness.Dto.Forms
{

    /// <summary>
    /// Represents the pagination, search, and sorting parameters for a list or table.
    /// </summary>
    public class PaginationSearchSortingParams
    {
        /// <summary>
        /// The current page number.
        /// </summary>
        /// <value>The default value is 1.</value>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// The number of items to display per page.
        /// </summary>
        /// <value>The default value is 10.</value>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// A search term that the user can enter.
        /// </summary>
        /// <value>The maximum length is 255 characters. Can be null.</value>
        [StringLength(255)]
        public string? Search { get; set; }

        /// <summary>
        /// The field to sort by.
        /// </summary>
        /// <value>The maximum length is 20 characters. Can be null.</value>
        [StringLength(20)]
        public string? SortBy { get; set; }

        /// <summary>
        /// Determines whether the sorting should be in descending order.
        /// </summary>
        /// <value>The default value is false, which means the sorting will be in ascending order.</value>
        public bool SortByDesc { get; set; } = false;
    }
}
