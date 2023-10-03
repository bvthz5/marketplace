using MarketPlaceAdmin.Bussiness.Dto.Forms.Validations;

namespace MarketPlaceAdmin.Bussiness.Dto.Forms
{
    /// <summary>
    /// Represents the parameters used to paginate, search and sort agent orders.
    /// </summary>
    public class AgentOrderPaginationParams : PaginationSearchSortingParams
    {
        /// <summary>
        /// The order status(es) to filter by.
        /// </summary>
        public List<byte?>? OrderStatus { get; set; }

        /// <summary>
        /// The zip code to search for.
        /// </summary>
        /// <remarks>
        /// If this property is null or empty, it indicates that the user does not want to filter by zip code.
        /// </remarks>
        [ZipCode(IsNullable = true)]
        public new string? Search { get; set; }

        /// <summary>
        /// A value indicating whether to include only the agent's own products.
        /// </summary>
        public bool MyProductsOnly { get; set; }

    }
}
