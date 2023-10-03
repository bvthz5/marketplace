using System.ComponentModel.DataAnnotations;

namespace MarketPlaceUser.Bussiness.Dto.Forms
{
    public class PaginationSearchSortingParams
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        [StringLength(255)]
        public string? Search { get; set; }

        [StringLength(20)]
        public string? SortBy { get; set; }

        public bool SortByDesc { get; set; } = false;
    }
}
