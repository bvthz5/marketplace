using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Bussiness.Dto.Forms;
using MarketPlaceUser.Bussiness.Dto.Views;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Services;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MarketPlaceUserTest.Servicetest
{
    public class DeliveryAddressServiceTest
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DeliveryAddressService _deliveryAddressService;

        public DeliveryAddressServiceTest()
        {
            // Create a substitute for the IUnitOfWork interface
            _unitOfWork = Substitute.For<IUnitOfWork>();

            // Create a new instance of the CartService class, passing the IUnitOfWork substitute as a constructor argument
            _deliveryAddressService = new DeliveryAddressService(_unitOfWork);
        }
        [Fact]
        public async Task AddAddressAsync_Should_AddAddress_And_ReturnServiceResult_WithNewAddress()
        {
            // Arrange
            var userId = 1;
            var deliveryAddressForm = new DeliveryAddressForm()
            {
                Address = "123 Main St",
                StreetAddress = "",
                City = "Anytown",
                State = "CA",
                Name = "John Doe",
                ZipCode = "12345",
                Phone = "555-555-1234"
            };
            var deliveryAddress = new DeliveryAddress()
            {
                Address = deliveryAddressForm.Address,
                StreetAddress = deliveryAddressForm.StreetAddress,
                City = deliveryAddressForm.City,
                State = deliveryAddressForm.State,
                Name = deliveryAddressForm.Name,
                ZipCode = deliveryAddressForm.ZipCode,
                Phone = deliveryAddressForm.Phone,
                CreatedUserId = userId,
                Status = DeliveryAddress.DeliveryAddressStatus.ACTIVE
            };
            _unitOfWork.DeliveryAddressRepository.FindByUserIdAndStatusNotRemovedAsync(userId).Returns(Task.FromResult(new List<DeliveryAddress>()));
            _unitOfWork.DeliveryAddressRepository.Add(Arg.Any<DeliveryAddress>()).Returns(deliveryAddress);

            // Act
            var result = await _deliveryAddressService.AddAddressAsync(deliveryAddressForm, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.NotNull(result.Data);
            var address = result.Data as DeliveryAddressView;
            Assert.NotNull(address);
            Assert.Equal(deliveryAddress.Address, address.Address);
            Assert.Equal(deliveryAddress.StreetAddress, address.StreetAddress);
            Assert.Equal(deliveryAddress.City, address.City);
            Assert.Equal(deliveryAddress.State, address.State);
            Assert.Equal(deliveryAddress.Name, address.Name);
            Assert.Equal(deliveryAddress.ZipCode, address.ZipCode);
            Assert.Equal(deliveryAddress.Phone, address.PhoneNumber);
            Assert.True(result.Status);
        }

        [Fact]
        public async Task AddAddressAsync_addressLimitExceeded_ReturnBadRequest()
        {
            // Arrange
            var userId = 1;
            var deliveryAddressForm = new DeliveryAddressForm()
            {
                Address = "123 Main St",
                StreetAddress = "",
                City = "Anytown",
                State = "CA",
                Name = "John Doe",
                ZipCode = "12345",
                Phone = "555-555-1234"
            };

            List<DeliveryAddress> addressList = Enumerable.Range(1, 5).Select(_ => new DeliveryAddress() { }).ToList();

            _unitOfWork.DeliveryAddressRepository.FindByUserIdAndStatusNotRemovedAsync(userId).Returns(addressList);

            // Act
            var result = await _deliveryAddressService.AddAddressAsync(deliveryAddressForm, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Address limit exceeded", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task GetAddressAsync_ReturnsServiceResultWithDeliveryAddresses()
        {
            // Arrange
            int userId = 1;
            var addresses = new List<DeliveryAddress>()
            {
                new DeliveryAddress() { DeliveryAddressId = 1, Name = "Address 1", Status = DeliveryAddress.DeliveryAddressStatus.ACTIVE },
                new DeliveryAddress() { DeliveryAddressId = 2, Name = "Address 2", Status = DeliveryAddress.DeliveryAddressStatus.REMOVED },
                new DeliveryAddress() { DeliveryAddressId = 3, Name = "Address 3", Status = DeliveryAddress.DeliveryAddressStatus.ACTIVE },
            };

            _unitOfWork.DeliveryAddressRepository.FindByUserIdAsync(userId).Returns(Task.FromResult(addresses));
            var service = new DeliveryAddressService(_unitOfWork);

            // Act
            var result = await service.GetAddressAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.NotNull(result.Data);
            Assert.IsType<List<DeliveryAddressView>>(result.Data);
            Assert.True(result.Status);
        }

        [Fact]
        public async Task GetAddressAsync_ReturnsServiceResultNotFound()
        {
            // Arrange
            int userId = 1;
            var addresses = new List<DeliveryAddress>();
            _unitOfWork.DeliveryAddressRepository.FindByUserIdAsync(userId).Returns(Task.FromResult(addresses));
            var service = new DeliveryAddressService(_unitOfWork);

            // Act
            var result = await service.GetAddressAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Address not found", result.Message);
            Assert.Null(result.Data);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task SetAddressDefault_WhenValidUserIdAndAddressId_ReturnsSuccessResult()
        {
            // Arrange
            int userId = 1;
            int addressId = 2;
            DeliveryAddress deliveryAddress = new() { DeliveryAddressId = addressId, CreatedUserId = userId, Status = DeliveryAddress.DeliveryAddressStatus.ACTIVE };
            DeliveryAddress defaultAddress = new() { DeliveryAddressId = 3, CreatedUserId = userId, Status = DeliveryAddress.DeliveryAddressStatus.DEFAULT };
            _unitOfWork.DeliveryAddressRepository.FindByUserIdAndAddressIdAsync(userId, addressId).Returns(deliveryAddress);
            _unitOfWork.DeliveryAddressRepository.FindByUserIdAndStatusAsync(userId, DeliveryAddress.DeliveryAddressStatus.DEFAULT).Returns(defaultAddress);

            // Act
            ServiceResult result = await _deliveryAddressService.SetAddressDefault(userId, addressId);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Status Changed", result.Message);
            Assert.True(result.Status);
        }

        [Fact]
        public async Task SetAddressDefault_WhenInvalidAddress_ReturnsNotFoundResult()
        {
            // Arrange
            int userId = 1;
            int addressId = 2;
            _unitOfWork.DeliveryAddressRepository.FindByUserIdAndAddressIdAsync(userId, addressId).ReturnsNull();

            // Act
            ServiceResult result = await _deliveryAddressService.SetAddressDefault(userId, addressId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Address not found", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task SetAddressDefault_WhenAddressAlreadyDefault_ReturnsSuccessResult()
        {
            // Arrange
            int userId = 1;
            int addressId = 2;
            DeliveryAddress deliveryAddress = new DeliveryAddress { DeliveryAddressId = addressId, CreatedUserId = userId, Status = DeliveryAddress.DeliveryAddressStatus.DEFAULT };
            _unitOfWork.DeliveryAddressRepository.FindByUserIdAndAddressIdAsync(userId, addressId).Returns(deliveryAddress);

            // Act
            ServiceResult result = await _deliveryAddressService.SetAddressDefault(userId, addressId);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Status Changed", result.Message);
            Assert.True(result.Status);
        }

        [Fact]
        public async Task GetAddressById_WithExistingAddress_ReturnsServiceResultWithData()
        {
            // Arrange
            int deliveryAddressId = 1;
            int userId = 1;
            DeliveryAddress address = new()
            {
                DeliveryAddressId = deliveryAddressId,
                CreatedUserId = userId
            };
            _unitOfWork.DeliveryAddressRepository.FindByUserIdAndAddressIdAsync(userId, deliveryAddressId).Returns(address);

            // Act
            ServiceResult result = await _deliveryAddressService.GetAddressById(deliveryAddressId, userId);

            // Assert
            Assert.NotNull(result.Data);
            Assert.IsType<DeliveryAddressView>(result.Data);
            Assert.Equal(address.DeliveryAddressId, ((DeliveryAddressView)result.Data).DeliveryAddressId);
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.True(result.Status);
        }

        [Fact]
        public async Task GetAddressById_AddressIsNull_ReturnsNotFound()
        {
            // Arrange
            int deliveryAddressId = 1;
            int userId = 1;
            _unitOfWork.DeliveryAddressRepository.FindByUserIdAndAddressIdAsync(userId, deliveryAddressId).ReturnsNull();

            // Act
            ServiceResult result = await _deliveryAddressService.GetAddressById(deliveryAddressId, userId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("No Address Found", result.Message);
            Assert.False(result.Status);
        }


        [Fact]
        public async Task EditAddressAsync_ReturnsNotFound_WhenAddressNotFound()
        {
            // Arrange

            DeliveryAddressForm deliveryAddressForm = new()
            {
                Address = "123 Main St",
                StreetAddress = "Apt 1",
                City = "Anytown",
                State = "CA",
                Name = "John Smith",
                ZipCode = "12345",
                Phone = "555-555-5555"
            };

            int deliveryAddressId = 1;
            int userId = 1;

            _unitOfWork.DeliveryAddressRepository.FindByUserIdAndAddressIdAsync(userId, deliveryAddressId).ReturnsNull();

            // Act
            var result = await _deliveryAddressService.EditAddressAsync(deliveryAddressForm, deliveryAddressId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Address not found", result.Message);
            Assert.Null(result.Data);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task EditAddressAsync_UpdatesAddress_WhenAddressFound()
        {
            // Arrange


            DeliveryAddressForm deliveryAddressForm = new DeliveryAddressForm
            {
                Address = "123 Main St",
                StreetAddress = "Apt 1",
                City = "Anytown",
                State = "CA",
                Name = "John Smith",
                ZipCode = "12345",
                Phone = "555-555-5555"
            };

            int deliveryAddressId = 1;
            int userId = 1;

            DeliveryAddress existingAddress = new DeliveryAddress
            {
                DeliveryAddressId = deliveryAddressId,
                CreatedUserId = userId,
                Address = "456 Elm St",
                StreetAddress = "Unit 2",
                City = "Othertown",
                State = "NY",
                Name = "Jane Doe",
                ZipCode = "67890",
                Phone = "555-123-4567",
                Status = DeliveryAddress.DeliveryAddressStatus.ACTIVE
            };

            DeliveryAddress updatedAddress = new()
            {
                DeliveryAddressId = deliveryAddressId,
                CreatedUserId = userId,
                Address = "123 Main St",
                StreetAddress = "Apt 1",
                City = "Anytown",
                State = "CA",
                Name = "John Smith",
                ZipCode = "12345",
                Phone = "555-555-5555",
                Status = DeliveryAddress.DeliveryAddressStatus.ACTIVE
            };

            _unitOfWork.DeliveryAddressRepository.FindByUserIdAndAddressIdAsync(userId, deliveryAddressId).Returns(existingAddress);

            _unitOfWork.DeliveryAddressRepository.Update(Arg.Any<DeliveryAddress>()).Returns(updatedAddress);

            // Act
            var result = await _deliveryAddressService.EditAddressAsync(deliveryAddressForm, deliveryAddressId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.NotNull(result.Data);

            DeliveryAddressView updatedAddressView = (DeliveryAddressView)result.Data;
            Assert.Equal(deliveryAddressForm.Address, updatedAddressView.Address);
            Assert.Equal(deliveryAddressForm.StreetAddress, updatedAddressView.StreetAddress);
            Assert.Equal(deliveryAddressForm.City, updatedAddressView.City);
            Assert.Equal(deliveryAddressForm.State, updatedAddressView.State);
            Assert.Equal(deliveryAddressForm.Name, updatedAddressView.Name);
            Assert.Equal(deliveryAddressForm.ZipCode, updatedAddressView.ZipCode);
            Assert.Equal(deliveryAddressForm.Phone, updatedAddressView.PhoneNumber);
            Assert.True(result.Status);
        }

        [Fact]
        public async Task DeleteAddress_AddressNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            int userId = 1;
            int deliveryAddressId = 1;

            // Set up the mock FindByUserIdAndAddressIdAsync method to return null
            _unitOfWork.DeliveryAddressRepository.FindByUserIdAndAddressIdAsync(userId, deliveryAddressId)
                .ReturnsNull();

            // Act
            var result = await _deliveryAddressService.DeleteAddress(userId, deliveryAddressId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Address not found", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task DeleteAddress_AddressAlreadyRemoved_ReturnsNotFoundResult()
        {
            // Arrange
            int userId = 1;
            int deliveryAddressId = 1;

            // Set up the mock FindByUserIdAndAddressIdAsync method to return a DeliveryAddress with Status = REMOVED
            _unitOfWork.DeliveryAddressRepository.FindByUserIdAndAddressIdAsync(userId, deliveryAddressId)
                .Returns(new DeliveryAddress { DeliveryAddressId = deliveryAddressId, Status = DeliveryAddress.DeliveryAddressStatus.REMOVED });

            // Act
            var result = await _deliveryAddressService.DeleteAddress(userId, deliveryAddressId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Address not found", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task DeleteAddress_DefaultAddress_ReturnsBadRequestResult()
        {
            // Arrange
            int userId = 1;
            int deliveryAddressId = 1;

            // Set up the mock FindByUserIdAndAddressIdAsync method to return a DeliveryAddress with Status = DEFAULT
            _unitOfWork.DeliveryAddressRepository.FindByUserIdAndAddressIdAsync(userId, deliveryAddressId)
                .Returns(new DeliveryAddress { DeliveryAddressId = deliveryAddressId, Status = DeliveryAddress.DeliveryAddressStatus.DEFAULT });

            // Act
            var result = await _deliveryAddressService.DeleteAddress(userId, deliveryAddressId);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Default address can't be deleted", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task DeleteAddress_ValidAddress_ReturnsSuccessResult()
        {
            // Arrange
            int userId = 1;
            int deliveryAddressId = 1;

            // Set up the mock FindByUserIdAndAddressIdAsync method to return a DeliveryAddress with Status = ACTIVE
            _unitOfWork.DeliveryAddressRepository.FindByUserIdAndAddressIdAsync(userId, deliveryAddressId)
                .Returns(new DeliveryAddress { DeliveryAddressId = deliveryAddressId, Status = DeliveryAddress.DeliveryAddressStatus.ACTIVE });

            // Act
            var result = await _deliveryAddressService.DeleteAddress(userId, deliveryAddressId);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Address deleted", result.Message);
            Assert.True(result.Status);
        }

        [Fact]
        public async Task ChangeStatus_UpdatesAddressStatus()
        {
            // Arrange
            var address = new DeliveryAddress { DeliveryAddressId = 1, Status = 0, Name = "Bv", Address = "Thz", City = "ktm" };
            var status = DeliveryAddress.DeliveryAddressStatus.DEFAULT;

            // Act
            await _deliveryAddressService.ChangeStatus(address, status);

            // Assert
            Assert.Equal(status, address.Status);
        }

        [Fact]
        public async Task ChangeStatus_ThrowsExceptionWhenAddressIsNull()
        {
            // Arrange
            DeliveryAddress? address = null;
            var status = DeliveryAddress.DeliveryAddressStatus.DEFAULT;

            // Act and Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _deliveryAddressService.ChangeStatus(address, status));
        }

        [Fact]
        public void Valid_DeliveryAddressConverter_ReturnsExpectedResult()
        {
            // Arrange
            var deliveryAddress = new DeliveryAddress
            {
                Name = "John Doe",
                Address = "123 Main St.",
                StreetAddress = "Apt. 4",
                City = "Anytown",
                ZipCode = "12345",
                State = "CA",
                Phone = "555-1234"
            };

            var expected = "John Doe\b123 Main St.\bApt. 4\bAnytown\b12345\bCA\b555-1234";

            var service = _deliveryAddressService;

            // Act
            var result = service.DeliveryAddressConverter(deliveryAddress);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void DeliveryAddress_object_null_empty_properties()
        {
            DeliveryAddress address = new DeliveryAddress()
            {
                Name = "",
                Address = null,
                StreetAddress = "Unit 3",
                City = "",
                ZipCode = "12345",
                State = null,
                Phone = "555-1234"
            };

            string expected = "\b\bUnit 3\b\b12345\b\b555-1234";
            string result = _deliveryAddressService.DeliveryAddressConverter(address);

            Assert.Equal(expected, result);

        }

        [Fact]
        public void DeliveryAddress_object_nonalphanumeric_characters()
        {
            DeliveryAddress address = new DeliveryAddress()
            {
                Name = "Mary's Place",
                Address = "1st Ave & 2nd St",
                StreetAddress = "",
                City = "New York",
                ZipCode = "10001-1234",
                State = "NY",
                Phone = "1-800-555-1212"
            };

            string expected = "Mary's Place\b1st Ave & 2nd St\b\bNew York\b10001-1234\bNY\b1-800-555-1212";
            string result = _deliveryAddressService.DeliveryAddressConverter(address);

            Assert.Equal(expected, result);

        }

        [Fact]
        public void DeliveryAddress_special_characters()
        {
            DeliveryAddress address = new DeliveryAddress()
            {
                Name = "Jürgen Müller",
                Address = "Am Bahnhof 1",
                StreetAddress = null,
                City = "Zürich",
                ZipCode = "8001",
                State = "",
                Phone = "+41 44 123 45 67"
            };

            string expected = "Jürgen Müller\bAm Bahnhof 1\b\bZürich\b8001\b\b+41 44 123 45 67";
            string result =_deliveryAddressService.DeliveryAddressConverter(address);

            Assert.Equal(expected, result);

        }
    }
}
