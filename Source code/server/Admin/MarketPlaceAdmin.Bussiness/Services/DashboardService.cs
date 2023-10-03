using MarketPlace.DataAccess.Interfaces;
using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Dto.Views;
using MarketPlaceAdmin.Bussiness.Enums;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using System.Globalization;

namespace MarketPlaceAdmin.Bussiness.Services
{
    /// <summary>
    /// Represents a service for generating dashboard data.
    /// </summary>
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _uow;

        /// <summary>
        /// Initializes a new instance of the DashboardService class with the specified unit of work.
        /// </summary>
        /// <param name="unitOfWork">The unit of work to use for data access.</param>
        public DashboardService(IUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }

        /// <summary>
        /// Returns the active product count under different categories. If the count is zero, the category will be ignored.
        /// </summary>
        /// <returns>
        /// Returns a ServiceResult object containing the CountView object with the following properties: <br/>
        /// - PropertyName: The name of the category. <br/>
        /// - Count: The number of active products in the category. <br/>
        /// </returns>
        public async Task<ServiceResult> GetActiveProductCountGroupByCategory()
        {
            Dictionary<string, int> categoryProductCount = await _uow.ProductRepostory.GetActiveProductCountGroupByCategory();

            // Returns a ServiceResult object with the count of active products in each category.
            return new ServiceResult()
            {
                ServiceStatus = ServiceStatus.Success,
                Message = "Category Wise Product Count",
                Data = categoryProductCount.Select(kv => new CountView(kv.Key, kv.Value))
            };
        }

        /// <summary>
        /// Returns the number of sold products per day within the specified date range.
        /// </summary>
        /// <param name="form">The FromToDateForm object containing the from and to dates for filtering.</param>
        /// <returns>
        /// Returns a ServiceResult object containing a CountView object with the following properties: <br/>
        /// - PropertyName: The date on which products were sold. <br/>
        /// - Count: The number of products sold on the specified date. <br/>
        /// </returns>
        public async Task<ServiceResult> GetSalesCount(FromToDateForm form)
        {
            ServiceResult result = new();

            // Validate the FromToDateForm object's from and to dates
            if (!DateTime.TryParseExact(form.From, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime from) ||
               !DateTime.TryParseExact(form.To, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime to))
            {
                result.Message = "Invalid Date";
                result.ServiceStatus = ServiceStatus.BadRequest;
                return result;
            }

            // Ensure that the To date is greater than or equal to the From date
            if (to < from)
            {
                result.Message = "From Date should be less than To date";
                result.ServiceStatus = ServiceStatus.BadRequest;
                return result;
            }

            // Create an array of dates within the specified range
            IEnumerable<DateTime> days = Enumerable.Range(0, (to - from).Days + 1).Select(day => from.AddDays(day).Date);

            // Get the count of sold products for each day within the specified range
            Dictionary<DateTime, int> salesCount = await _uow.OrderDetailsRepository.GetSalesCount(from, to);

            // Create a new ServiceResult object with the sales count data
            result.ServiceStatus = ServiceStatus.Success;
            result.Message = "Sales Count";

            // Fill the array of dates created with actual data, leaving a count of 0 for dates without any sales
            result.Data = days.Select(date => new CountView(date.Date.ToShortDateString(), salesCount.TryGetValue(date.Date, out int count) ? count : 0));

            return result;
        }
    }
}
