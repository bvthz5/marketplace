namespace MarketPlaceAdmin.Bussiness.Dto.Forms
{

    /// <summary>
    /// Represents the pagination parameters for users, including search and sorting options.
    /// Inherits from the <see cref="PaginationSearchSortingParams"/> class.
    /// </summary>
    public class UserPaginationParams : PaginationSearchSortingParams
    {
        /// <summary>
        /// The status(es) to filter by.
        /// </summary>
        public List<byte?>? Status { get; set; }

        /// <summary>
        /// The role(s) to filter by.
        /// </summary>
        public List<byte?>? Role { get; set; }
    }
}
