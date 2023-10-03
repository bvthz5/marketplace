using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Bussiness.Dto.Forms;
using MarketPlaceUser.Bussiness.Dto.Views;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;

namespace MarketPlaceUser.Bussiness.Services
{
    public class DeliveryAddressService : IDeliveryAddressService
    {
        public readonly IUnitOfWork _uow;

        public DeliveryAddressService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        /// <summary>
        /// Adds a new delivery address for the specified user.
        /// </summary>
        /// <param name="deliveryAddressForm">A <see cref="DeliveryAddressForm"/> object containing the delivery address data.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a <see cref="ServiceResult"/> object.</returns>
        public async Task<ServiceResult> AddAddressAsync(DeliveryAddressForm deliveryAddressForm, int userId)
        {
            // Initialize a new ServiceResult object.
            ServiceResult result = new();

            // Find all delivery addresses that are not marked as removed for the specified user.
            var address = await _uow.DeliveryAddressRepository.FindByUserIdAndStatusNotRemovedAsync(userId);

            // If the user has already added the maximum number of delivery addresses, return a BadRequest result.
            if (address.Count == 5)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Address limit exceeded";
                return result;
            }

            // Create a new DeliveryAddress object from the data in the DeliveryAddressForm object, and set the CreatedUserId and Status properties.
            var deliveryAddress = new DeliveryAddress()
            {
                Address = deliveryAddressForm.Address.Trim(),
                StreetAddress = deliveryAddressForm.StreetAddress.Trim(),
                City = deliveryAddressForm.City.Trim(),
                State = deliveryAddressForm.State.Trim(),
                Name = deliveryAddressForm.Name.Trim(),
                ZipCode = deliveryAddressForm.ZipCode.Trim(),
                Phone = deliveryAddressForm.Phone.Trim(),
                CreatedUserId = userId,
                Status = DeliveryAddress.DeliveryAddressStatus.ACTIVE
            };

            // Add the new delivery address to the repository and save changes.
            await _uow.DeliveryAddressRepository.Add(deliveryAddress);
            await _uow.SaveAsync();

            // Set the new delivery address as the default address for the user.
            await SetAddressDefault(userId, deliveryAddress.DeliveryAddressId);

            // Create a new DeliveryAddressView object from the new delivery address, and set the Data property of the ServiceResult object.
            result.Data = new DeliveryAddressView(deliveryAddress);

            return result;
        }


        /// <summary>
        /// Retrieves the delivery addresses associated with a specified user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a <see cref="ServiceResult"/> object.</returns>
        public async Task<ServiceResult> GetAddressAsync(int userId)
        {
            // Initialize a new ServiceResult object.
            ServiceResult result = new();

            // Find all delivery addresses associated with the specified user.
            var address = await _uow.DeliveryAddressRepository.FindByUserIdAsync(userId);

            // If no delivery addresses are found, return a NotFound result.
            if (address.Count == 0)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Address not found";
                return result;
            }

            // Filter out delivery addresses that have been removed, and create a new list of DeliveryAddressView objects from the remaining delivery addresses.
            result.Data = address.Where(address => address.Status != DeliveryAddress.DeliveryAddressStatus.REMOVED)
                                 .ToList()
                                 .ConvertAll(adr => new DeliveryAddressView(adr));

            return result;
        }


        /// <summary>
        /// Sets the default delivery address for a given user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="deliveryAddressId">The ID of the delivery address.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a <see cref="ServiceResult"/> object.</returns>
        public async Task<ServiceResult> SetAddressDefault(int userId, int deliveryAddressId)
        {
            // Initialize a new ServiceResult object.
            ServiceResult result = new();

            // Find the delivery address associated with the specified user ID and delivery address ID.
            DeliveryAddress? deliveryAddress = await _uow.DeliveryAddressRepository.FindByUserIdAndAddressIdAsync(userId, deliveryAddressId);

            // If the delivery address is not found or has been removed, return a NotFound result.
            if (deliveryAddress == null || deliveryAddress.Status == DeliveryAddress.DeliveryAddressStatus.REMOVED)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Address not found";
                return result;
            }

            // Find the delivery address that is currently set as the default for the specified user.
            DeliveryAddress? address = _uow.DeliveryAddressRepository.FindByUserIdAndStatusAsync(userId, DeliveryAddress.DeliveryAddressStatus.DEFAULT);

            // If there is a delivery address that is currently set as default, set its status to ACTIVE.
            if (address != null)
            {
                await ChangeStatus(address: address, DeliveryAddress.DeliveryAddressStatus.ACTIVE);
            }

            // Set the status of the specified delivery address to DEFAULT.
            await ChangeStatus(deliveryAddress, DeliveryAddress.DeliveryAddressStatus.DEFAULT);

            // Set the message of the result to "Status Changed".
            result.Message = "Status Changed";
            return result;
        }


        /// <summary>
        /// Changes the status of the given delivery address and saves the changes to the database.
        /// </summary>
        /// <param name="address">The delivery address to update.</param>
        /// <param name="status">The new status to set for the delivery address.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ChangeStatus(DeliveryAddress address, DeliveryAddress.DeliveryAddressStatus status)
        {
            address.Status = status;
            _uow.DeliveryAddressRepository.Update(address);

            await _uow.SaveAsync();
        }

        /// <summary>
        /// Gets a delivery address by ID and user ID.
        /// </summary>
        /// <param name="deliveryAddressId">The ID of the delivery address to retrieve.</param>
        /// <param name="userId">The ID of the user who owns the delivery address.</param>
        /// <returns>A <see cref="ServiceResult"/> containing a <see cref="DeliveryAddressView"/> if the address is found, or an error message if not.</returns>
        public async Task<ServiceResult> GetAddressById(int deliveryAddressId, int userId)
        {
            ServiceResult result = new();

            // Find the delivery address by ID and user ID
            DeliveryAddress? address = await _uow.DeliveryAddressRepository.FindByUserIdAndAddressIdAsync(userId, deliveryAddressId);

            // If the address is not found, return a not found error message
            if (address is null)
            {
                result.Message = "No Address Found";
                result.ServiceStatus = ServiceStatus.NotFound;

                return result;
            }

            // Otherwise, return the delivery address as a DeliveryAddressView
            result.Data = new DeliveryAddressView(address);
            return result;
        }


        /// <summary>
        /// Edits the specified delivery address for the given user ID.
        /// </summary>
        /// <param name="deliveryAddressForm">The form containing the updated delivery address information.</param>
        /// <param name="deliveryAddressId">The ID of the delivery address to edit.</param>
        /// <param name="userId">The ID of the user who owns the delivery address.</param>
        /// <returns>A service result indicating the success or failure of the operation, along with the updated delivery address data if successful.</returns>
        public async Task<ServiceResult> EditAddressAsync(DeliveryAddressForm deliveryAddressForm, int deliveryAddressId, int userId)
        {
            ServiceResult result = new();

            // Find the delivery address with the specified ID and ensure it is not marked as removed
            DeliveryAddress? address = await _uow.DeliveryAddressRepository.FindByUserIdAndAddressIdAsync(userId, deliveryAddressId);

            if (address == null || address.Status == DeliveryAddress.DeliveryAddressStatus.REMOVED)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Address not found";

                return result;
            }

            // Update the delivery address with the new information
            address.Address = deliveryAddressForm.Address.Trim();
            address.StreetAddress = deliveryAddressForm.StreetAddress.Trim();
            address.City = deliveryAddressForm.City.Trim();
            address.State = deliveryAddressForm.State.Trim();
            address.Name = deliveryAddressForm.Name.Trim();
            address.ZipCode = deliveryAddressForm.ZipCode.Trim();
            address.Phone = deliveryAddressForm.Phone.Trim();

            // Save the updated delivery address to the database
            result.Data = new DeliveryAddressView(_uow.DeliveryAddressRepository.Update(address));
            await _uow.SaveAsync();

            // Set the edited address as the user's default address
            await SetAddressDefault(userId, address.DeliveryAddressId);

            return result;
        }


        /// <summary>
        /// Deletes a delivery address for a given user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="deliveryAddressId">The delivery address id.</param>
        /// <returns>A service result indicating the outcome of the operation.</returns>
        public async Task<ServiceResult> DeleteAddress(int userId, int deliveryAddressId)
        {
            ServiceResult result = new();

            // Find the delivery address to be deleted
            var address = await _uow.DeliveryAddressRepository.FindByUserIdAndAddressIdAsync(userId, deliveryAddressId);

            // If the address is not found or has already been removed, return an error
            if (address == null || address.Status == DeliveryAddress.DeliveryAddressStatus.REMOVED)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Address not found";
                return result;
            }

            // If the address is the default one, it cannot be deleted
            if (address.Status == DeliveryAddress.DeliveryAddressStatus.DEFAULT)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Default address can't be deleted";
                return result;
            }

            // Mark the address as removed and update the repository
            address.Status = DeliveryAddress.DeliveryAddressStatus.REMOVED;
            _uow.DeliveryAddressRepository.Update(address);
            await _uow.SaveAsync();

            // Return a success message
            result.Message = "Address deleted";
            return result;
        }

        public string DeliveryAddressConverter(DeliveryAddress deliveryAddress)
        {
            string address = $"{deliveryAddress.Name}{'\b'}{deliveryAddress.Address}{'\b'}{deliveryAddress.StreetAddress}{'\b'}{deliveryAddress.City}{'\b'}{deliveryAddress.ZipCode}{'\b'}{deliveryAddress.State}{'\b'}{deliveryAddress.Phone}";

            return address;
        }
    }
}
