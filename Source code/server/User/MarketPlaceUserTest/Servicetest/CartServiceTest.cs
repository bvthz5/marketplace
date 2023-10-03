using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Bussiness.Dto.Views;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Services;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MarketPlaceUserTest.Servicetest
{
    public class CartServiceTest
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly CartService _cartService;

        public CartServiceTest()
        {
            // Create a substitute for the IUnitOfWork interface
            _unitOfWork = NSubstitute.Substitute.For<IUnitOfWork>();

            // Create a new instance of the CartService class, passing the IUnitOfWork substitute as a constructor argument
            _cartService = new CartService(_unitOfWork);
        }

        [Fact]
        public async Task AddToCartAsync_ProductNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            var userId = 1;
            var productId = 1;
            _unitOfWork.ProductRepostory.FindById(productId).ReturnsNull();

            // Act
            var result = await _cartService.AddToCartAsync(userId, productId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Product Not Found", result.Message);
            Assert.False(result.Status);
        }


        [Fact]
        public async Task AddToCartAsync_ProductIsNotActive_ReturnsNotFoundResult()
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
            var result = await _cartService.AddToCartAsync(userId, productId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Product Not Found", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task AddToCartAsync_CreatedUserIsNotActive_ReturnsNotFoundResult()
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
            var result = await _cartService.AddToCartAsync(userId, productId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Product Not Found", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task AddToCartAsync_CreatedUserIsAsking_ReturnsNotFoundResult()
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
            var result = await _cartService.AddToCartAsync(userId, productId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Product Not Found", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task AddToCartAsync_CarttLimitExceeded_ReturnsNotFoundResult()
        {
            // Arrange
            int userId = 1;
            int productId = 2;
            var cart = new Cart();
            List<Cart> cartList = Enumerable.Range(1, 50).Select(_ => cart).ToList();
            _unitOfWork.CartRepository.FindByUserIdAsync(userId).Returns(cartList);
            var product = new Product()
            {
                ProductId = productId,
                CreatedUserId = 2,
                Status = Product.ProductStatus.ACTIVE,
                CreatedUser = new User() { UserId = userId, Status = User.UserStatus.ACTIVE },
            };
            _unitOfWork.ProductRepostory.FindById(productId).Returns(product);

            // Act
            var result = await _cartService.AddToCartAsync(userId, productId);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Cart Max Limit (50) exceed", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task AddToCartAsync_ProductAlreadyExists_ReturnsAlreadyExists()
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

            var cart = new Cart()
            {
                CartId = 1,
                ProductId = productId,
                UserId = userId,
                Product = product
            };

            List<Cart> cartList = Enumerable.Range(1, 1).Select(_ => cart).ToList();
            _unitOfWork.CartRepository.FindByUserIdAsync(userId).Returns(cartList);

            _unitOfWork.ProductRepostory.FindById(productId).Returns(product);

            // Act
            var result = await _cartService.AddToCartAsync(userId, productId);

            // Assert
            Assert.Equal(ServiceStatus.AlreadyExists, result.ServiceStatus);
            Assert.Equal("Product Already Added", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task AddToCartAsync_Success()
        {
            // Arrange
            // Create a fake product and user IDs
            int productId = 123;
            int userId = 456;

            // Create a fake product and user
            var product = new Product
            {
                ProductId = productId,
                CreatedUserId = 789,
                Status = Product.ProductStatus.ACTIVE,
                CreatedUser = new User { Status = User.UserStatus.ACTIVE }
            };
            var user = new User { UserId = userId, Status = User.UserStatus.ACTIVE };

            // Create a fake UnitOfWork instance

            _unitOfWork.ProductRepostory.FindById(productId).Returns(product);
            _unitOfWork.CartRepository.FindByUserIdAsync(userId).Returns(new System.Collections.Generic.List<Cart>());
            _unitOfWork.SaveAsync().Returns(true);

            // Act
            var result = await _cartService.AddToCartAsync(userId, productId);

            // Assert
            // Check that the correct message is returned
            Assert.Equal("Product Added", result.Message);

            // Check that the correct ServiceStatus is returned
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);

            // Check that the CartRepository's AddAsync method was called with the correct Cart object
            await _unitOfWork.CartRepository.Received().Add(Arg.Is<Cart>(cart => cart.UserId == userId && cart.ProductId == productId));

            // Check that the UnitOfWork's SaveAsync method was called exactly once
            await _unitOfWork.Received(1).SaveAsync();
            Assert.True(result.Status);
        }

        [Fact]
        public async Task GetCartAsync_Success_ReturnsProductCartWishListViewList()
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
            var cartList = new List<Cart>()
    {
        new Cart()
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
            _unitOfWork.CartRepository.FindByUserIdAsync(userId).Returns(cartList);
            _unitOfWork.ProductRepostory.FindById(Arg.Any<int>())
                .Returns(product);
            _unitOfWork.PhotoRepository.FindThumbnailPicture(Arg.Any<int>()).Returns(photo);

            // Act
            var result = await _cartService.GetCartAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.True(result.Status);
            Assert.IsType<List<ProductCartWishListView>>(result.Data);
            var productDetailView = (List<ProductCartWishListView>)result.Data;
            Assert.Single(productDetailView);
            Assert.Equal(product.ProductId, productDetailView[0].ProductId);
            Assert.Equal(product.ProductName, productDetailView[0].ProductName);
            Assert.Equal(product.CategoryId, productDetailView[0].CategoryId);
            Assert.Equal(product.Category.CategoryName, productDetailView[0].CategoryName);
            Assert.Equal(product.ProductDescription, productDetailView[0].ProductDescription);
            Assert.Equal(product.Address, productDetailView[0].Address);
            Assert.Equal(product.Price, productDetailView[0].Price);
            Assert.Equal(product.CreatedDate, productDetailView[0].CreatedDate);
            Assert.Equal((byte)product.Status, productDetailView[0].Status);
            Assert.Equal((byte)product.CreatedUser.Status, productDetailView[0].CreatedUserStatus);
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
            var cartList = new List<Cart>()
            {
                new Cart()
                {
                    UserId = userId,
                    ProductId = 1,
                    Product = product
                }
            };


            _unitOfWork.CartRepository.FindByUserIdAsync(userId).Returns(cartList);
            _unitOfWork.ProductRepostory.FindById(Arg.Any<int>())
                .Returns(product);
            _unitOfWork.PhotoRepository.FindThumbnailPicture(Arg.Any<int>()).ReturnsNull();

            // Act
            var result = await _cartService.GetCartAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.True(result.Status);

        }

        [Fact]
        public async Task GetCartAsync_CartIsNull_ReturnsSuccess()
        {
            // Arrange
            var userId = 1;
            var cartList = new List<Cart>();

            _unitOfWork.CartRepository.FindByUserIdAsync(userId).Returns(cartList);
            _unitOfWork.ProductRepostory.FindById(Arg.Any<int>())
                .ReturnsNull();
            _unitOfWork.PhotoRepository.FindThumbnailPicture(Arg.Any<int>()).ReturnsNull();

            // Act
            var result = await _cartService.GetCartAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Cart is empty", result.Message);
            Assert.True(result.Status);

        }

        [Fact]
        public async Task RemoveFromCartAsync_ProductInCart_RemovesProduct()
        {
            // Arrange
            var userId = 1;
            var productId = 1;

            // Create a list with a single cart item that matches the userId and productId
            var cartList = new List<Cart>()
            {
                new Cart()
                {
                    UserId = userId,
                    ProductId = productId
                }
            };

            // Set up the mock method FindByProductIdAndUserIdAsync to return the cart item created above
            _unitOfWork.CartRepository.FindByProductIdAndUserIdAsync(productId, userId).Returns(cartList.FirstOrDefault());

            // Act
            var result = await _cartService.RemoveFromCartAsync(userId, productId);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Removed From Cart", result.Message);

            // Verify that the mock method DeleteByProductIdAndUserIdAsync was called with the expected parameters
            await _unitOfWork.CartRepository.Received(1).DeleteByProductIdAndUserIdAsync(productId, userId);

            // Verify that the mock method SaveAsync was called
            await _unitOfWork.Received(1).SaveAsync();
            Assert.True(result.Status);
        }

        [Fact]
        public async Task RemoveFromCartAsync_ProductNotInCart_ReturnsNotFound()
        {
            // Arrange
            var userId = 1;
            var productId = 1;

            // Set up the mock method FindByProductIdAndUserIdAsync to return null, indicating that the product is not in the cart
            _unitOfWork.CartRepository.FindByProductIdAndUserIdAsync(productId, userId).ReturnsNull();

            // Act
            var result = await _cartService.RemoveFromCartAsync(userId, productId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Product Not Found", result.Message);

            // Verify that the mock method DeleteByProductIdAndUserIdAsync was not called
            await _unitOfWork.CartRepository.DidNotReceive().DeleteByProductIdAndUserIdAsync(productId, userId);

            // Verify that the mock method SaveAsync was not called
            await _unitOfWork.DidNotReceive().SaveAsync(); 
            Assert.False(result.Status);
        }
    }

}

