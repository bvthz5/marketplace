using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Services;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MarketPlaceUserTest.Servicetest
{
    public class WishListServiceTest
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly WishListService _wishListService;

        public WishListServiceTest()
        {
            _unitOfWork = NSubstitute.Substitute.For<IUnitOfWork>();

            _wishListService = new WishListService(_unitOfWork);
        }

        [Fact]
        public async Task AddToWishlistAsync_ProductNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            var userId = 1;
            var productId = 1;
            _unitOfWork.ProductRepostory.FindById(productId).ReturnsNull();

            // Act
            var result = await _wishListService.AddToWishListAsync(userId, productId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Product Not Found", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task AddToWishlistAsync_ProductIsNotActive_ReturnsNotFoundResult()
        {
            // Arrange
            var userId = 1;
            var productId = 1;
            var product = new Product()
            {
                ProductId = productId,
                CreatedUserId = userId,
                Status = Product.ProductStatus.INACTIVE,
                CreatedUser = new User() { UserId = userId, Status = User.UserStatus.ACTIVE },
            };
            _unitOfWork.ProductRepostory.FindById(productId).Returns(product);

            // Act
            var result = await _wishListService.AddToWishListAsync(userId, productId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Product Not Found", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task AddToWishlistAsync_CreatedUserIsNotActive_ReturnsNotFoundResult()
        {
            // Arrange
            var userId = 1;
            var productId = 1;
            var product = new Product()
            {
                ProductId = productId,
                CreatedUserId = userId,
                Status = Product.ProductStatus.ACTIVE,
                CreatedUser = new User() { UserId = userId, Status = User.UserStatus.INACTIVE },
            };
            _unitOfWork.ProductRepostory.FindById(productId).Returns(product);

            // Act
            var result = await _wishListService.AddToWishListAsync(userId, productId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Product Not Found", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task AddToWishlistAsync_CreatedUserIsAsking_ReturnsNotFoundResult()
        {
            // Arrange
            var userId = 1;
            var productId = 1;
            var product = new Product()
            {
                ProductId = productId,
                CreatedUserId = userId,
                Status = Product.ProductStatus.ACTIVE,
                CreatedUser = new User() { UserId = userId, Status = User.UserStatus.ACTIVE },
            };
            _unitOfWork.ProductRepostory.FindById(productId).Returns(product);

            // Act
            var result = await _wishListService.AddToWishListAsync(userId, productId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Product Not Found", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task AddToWishlistAsync_ProductAlreadyExists_ReturnsAlreadyExists()
        {
            // Arrange
            int userId = 1;
            int productId = 2;

            var product = new Product()
            {
                ProductId = productId,
                CreatedUserId = 2,
                Status = Product.ProductStatus.ACTIVE,
                CreatedUser = new User() { UserId = userId, Status = User.UserStatus.ACTIVE },
            };

            var wishlist = new WishList()
            {
                WishListId = 1,
                ProductId = productId,
                UserId = userId,
                Product = product
            };

            List<WishList> wishLists = Enumerable.Range(1, 1).Select(_ => wishlist).ToList();
            _unitOfWork.WishListRepository.FindByProductIdAndUserIdAsync(productId, userId).Returns(wishlist);

            _unitOfWork.ProductRepostory.FindById(productId).Returns(product);

            // Act
            var result = await _wishListService.AddToWishListAsync(userId, productId);

            // Assert
            Assert.Equal(ServiceStatus.AlreadyExists, result.ServiceStatus);
            Assert.Equal("Product Already Added", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task AddToWishlistAsync_Success()
        {
            // Arrange
            int productId = 123;
            int userId = 456;

            var product = new Product
            {
                ProductId = productId,
                CreatedUserId = 789,
                Status = Product.ProductStatus.ACTIVE,
                CreatedUser = new User { Status = User.UserStatus.ACTIVE }
            };
            var user = new User { UserId = userId, Status = User.UserStatus.ACTIVE };


            _unitOfWork.ProductRepostory.FindById(productId).Returns(product);
            _unitOfWork.WishListRepository.FindByUserIdAsync(userId).Returns(new List<WishList>());
            _unitOfWork.SaveAsync().Returns(true);

            // Act
            var result = await _wishListService.AddToWishListAsync(userId, productId);

            // Assert
            Assert.Equal("Product Added", result.Message);

            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.True(result.Status);

            await _unitOfWork.WishListRepository.Received().Add(Arg.Is<WishList>(wishList => wishList.UserId == userId && wishList.ProductId == productId));

            await _unitOfWork.Received(1).SaveAsync();
        }


        [Fact]
        public async Task GetWishListAsync_Success_ReturnsProductCartWishListViewList()
        {
            // Arrange
            var userId = 1;
            var category = new Category()
            {
                CategoryId = 1,
                CategoryName = "Test",
            };
            var createdUser = new User()
            {
                UserId = 123,
                Status = User.UserStatus.ACTIVE
            };
            var product = new Product()
            {
                ProductId = 1,
                ProductName = "Product 1",
                ProductDescription = "Description 1",
                Price = 10000.0,
                Status = Product.ProductStatus.ACTIVE,
                CreatedUserId = 1,
                CategoryId = 1,
                Address = "Address",
                CreatedDate = DateTime.Now,
                Category = category,
                CreatedUser = createdUser

            };
            var wishLists = new List<WishList>()
            {
                new WishList()
                {
                    UserId = userId,
                    ProductId = 1,
                    Product = product
                }
            };

            var photo = new Photos()
            {
                PhotosId = 1,
                ProductId = 1,
                Photo = "ProductImage.png"

            };
            _unitOfWork.WishListRepository.FindByUserIdAsync(userId).Returns(wishLists);
            _unitOfWork.ProductRepostory.FindById(Arg.Any<int>())
                .Returns(product);
            _unitOfWork.PhotoRepository.FindThumbnailPicture(Arg.Any<int>()).Returns(photo);

            // Act
            var result = await _wishListService.GetWishListAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.True(result.Status);
           

        }

        [Fact]
        public async Task GetCartAsync_SuccesswhenPhotoIsNull_ReturnsProductCartWishListViewList()
        {
            // Arrange
            var userId = 1;
            var category = new Category()
            {
                CategoryId = 1,
                CategoryName = "Test",
            };
            var createdUser = new User()
            {
                UserId = 123,
                Status = User.UserStatus.ACTIVE
            };
            var product = new Product()
            {
                ProductId = 1,
                ProductName = "Product 1",
                ProductDescription = "Description 1",
                Price = 10000.0,
                Status = Product.ProductStatus.ACTIVE,
                CreatedUserId = 1,
                CategoryId = 1,
                Address = "Address",
                CreatedDate = DateTime.Now,
                Category = category,
                CreatedUser = createdUser

            };
            var wishLists = new List<WishList>()
            {
                new WishList()
                {
                    UserId = userId,
                    ProductId = 1,
                    Product = product
                }
            };


            _unitOfWork.WishListRepository.FindByUserIdAsync(userId).Returns(wishLists);
            _unitOfWork.ProductRepostory.FindById(Arg.Any<int>())
                .Returns(product);
            _unitOfWork.PhotoRepository.FindThumbnailPicture(Arg.Any<int>()).ReturnsNull();

            // Act
            var result = await _wishListService.GetWishListAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.True(result.Status);

        }

        [Fact]
        public async Task GetWishListAsync_WishListIsNull_ReturnsSuccess()
        {
            // Arrange
            var userId = 1;
            var wishLists = new List<WishList>();

            _unitOfWork.WishListRepository.FindByUserIdAsync(userId).Returns(wishLists);
            _unitOfWork.ProductRepostory.FindById(Arg.Any<int>())
                .ReturnsNull();
            _unitOfWork.PhotoRepository.FindThumbnailPicture(Arg.Any<int>()).ReturnsNull();

            // Act
            var result = await _wishListService.GetWishListAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Wishlist is empty", result.Message);
            Assert.True(result.Status);
        }

        [Fact]
        public async Task RemoveFromWishListAsync_ProductInWishlist_RemovesProduct()
        {
            // Arrange
            var userId = 1;
            var productId = 1;

            var wishLists = new List<WishList>()
            {
                new WishList()
                {
                    UserId = userId,
                    ProductId = productId
                }
            };

            _unitOfWork.WishListRepository.FindByProductIdAndUserIdAsync(productId, userId).Returns(wishLists.FirstOrDefault());

            // Act
            var result = await _wishListService.RemoveFromWishListAsync(userId, productId);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Removed From WishList", result.Message);
            Assert.True(result.Status);

            await _unitOfWork.WishListRepository.Received(1).DeleteByProductIdAndUserIdAsync(productId, userId);

            await _unitOfWork.Received(1).SaveAsync();
        }

        [Fact]
        public async Task RemoveFromWishListAsync_ProductNotInWishList_ReturnsNotFound()
        {
            // Arrange
            var userId = 1;
            var productId = 1;

            _unitOfWork.WishListRepository.FindByProductIdAndUserIdAsync(productId, userId).ReturnsNull();

            // Act
            var result = await _wishListService.RemoveFromWishListAsync(userId, productId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Product Not Found", result.Message);
            Assert.False(result.Status);

            await _unitOfWork.WishListRepository.DidNotReceive().DeleteByProductIdAndUserIdAsync(productId, userId);

            await _unitOfWork.DidNotReceive().SaveAsync();
        }
    }
}
