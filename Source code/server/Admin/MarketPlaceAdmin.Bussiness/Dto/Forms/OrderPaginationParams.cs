namespace MarketPlaceAdmin.Bussiness.Dto.Forms
{
    /// <summary>
    /// Represents the pagination parameters for orders, including search and sorting options.
    /// </summary>
    public class OrderPaginationParams : PaginationSearchSortingParams
    {
        /// <summary>
        /// The payment status(es) to filter by.
        /// </summary>
        public List<byte?>? PaymentStatus { get; set; }

        /// <summary>
        /// The order status(es) to filter by.
        /// </summary>
        public List<byte?>? OrderStatus { get; set; }

        /// <summary>
        /// The ID of the buyer associated with the orders.
        /// </summary>
        public int BuyerId { get; set; }
    }
}
