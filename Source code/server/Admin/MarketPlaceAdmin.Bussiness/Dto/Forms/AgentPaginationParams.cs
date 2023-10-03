namespace MarketPlaceAdmin.Bussiness.Dto.Forms
{
    public class AgentPaginationParams : PaginationSearchSortingParams
    {
        /// <summary>
        /// The status(es) to filter by.
        /// </summary>
        public List<byte?>? Status { get; set; }
    }
}
