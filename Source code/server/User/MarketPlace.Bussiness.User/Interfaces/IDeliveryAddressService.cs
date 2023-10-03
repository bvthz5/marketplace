using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Bussiness.Dto.Forms;
using MarketPlaceUser.Bussiness.Helper;

namespace MarketPlaceUser.Bussiness.Interfaces
{
    public interface IDeliveryAddressService
    {
        Task<ServiceResult> AddAddressAsync(DeliveryAddressForm deliveryAddressForm, int userId);

        Task<ServiceResult> GetAddressAsync(int userId);

        Task<ServiceResult> SetAddressDefault(int userId, int deliveryAddressId);

        Task<ServiceResult> GetAddressById(int deliveryAddressId, int userId);

        Task<ServiceResult> EditAddressAsync(DeliveryAddressForm deliveryAddressForm, int deliveryAddressId, int userId);

        Task<ServiceResult> DeleteAddress(int userId, int deliveryAddressId);

        string DeliveryAddressConverter(DeliveryAddress deliveryAddress);
    }
}
