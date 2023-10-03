using System.ComponentModel.DataAnnotations;

namespace MarketPlaceUser.Bussiness.Dto.Forms
{
    public class ProductPaginationParams
    {
        public int Offset { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        [StringLength(255)]
        public string? Search { get; set; }

        [StringLength(20)]
        public string? SortBy { get; set; }

        public bool SortByDesc { get; set; } = false;

        public int?[]? CategoryId { get; set; }

        [Range(0.00, 10000000.00)]
        public float StartPrice { get; set; } = 0;

        [Range(0.00, 10000000.00)]
        public float EndPrice { get; set; } = 0;

        [StringLength(255)]
        public string? Location { get; set; }

    }
}
