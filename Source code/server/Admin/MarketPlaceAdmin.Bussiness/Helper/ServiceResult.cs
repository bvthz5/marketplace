using MarketPlaceAdmin.Bussiness.Enums;

namespace MarketPlaceAdmin.Bussiness.Helper
{
    /// <summary>
    /// Represents the result of a service operation.
    /// </summary>
    public class ServiceResult
    {
        /// <summary>
        /// The data returned by the service operation.
        /// </summary>
        public object? Data { get; set; }

        /// <summary>
        /// The message associated with the service result.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// The status of the service result.
        /// </summary>
        public ServiceStatus ServiceStatus { get; set; } = ServiceStatus.Success;

        /// <summary>
        /// Gets a value indicating whether the service operation was successful.
        /// </summary>
        public bool Status
        {
            get => ServiceStatus == ServiceStatus.Success;
        }
    }
}
