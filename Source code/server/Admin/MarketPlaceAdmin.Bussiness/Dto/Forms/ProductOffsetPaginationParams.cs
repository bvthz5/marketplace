namespace MarketPlaceAdmin.Bussiness.Dto.Forms
{
    /// <summary>
    /// Represents the parameters used for paginating and filtering a list of products.
    /// </summary>
    public class ProductOffsetPaginationParams : ProductPaginationParams
    {
        /// <summary>
        /// The offset for the paginated results.
        /// </summary>
        /// <value>The offset value.</value>
        public int Offset { get; set; } = 0;

        /// <summary>
        /// Hides the page number property of PaginationSearchSortingParams
        /// </summary>
        private new int PageNumber { get; set; } = 1;

    }
}
