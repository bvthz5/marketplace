using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Helper;

namespace MarketPlaceAdmin.Bussiness.Interfaces
{
    public interface IDashboardService
    {
        Task<ServiceResult> GetActiveProductCountGroupByCategory();

        Task<ServiceResult> GetSalesCount(FromToDateForm form);

    }
}
