using MarketPlaceUser.Bussiness.Enums;

namespace MarketPlaceUser.Bussiness.Helper
{

    public class ServiceResult
    {
        public object? Data { get; set; }
        public string? Message { get; set; }
        public ServiceStatus ServiceStatus { get; set; } = ServiceStatus.Success;

        public bool Status
        {
            get => ServiceStatus == ServiceStatus.Success || ServiceStatus == ServiceStatus.Created;
        }
    }
}
