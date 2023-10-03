using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Bussiness.Dto.Forms;
using MarketPlaceUser.Bussiness.Dto.Views;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Interfaces;
using MarketPlaceUser.Bussiness.Security;
using MarketPlaceUser.Bussiness.Services;
using Moq;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MarketPlaceUserTest.Servicetest
{
    public class ProductServiceTest
    {

        private readonly IProductService _productService;
        private readonly IUnitOfWork _uow;
        private readonly ISecurityUtil _securityUtil;

        public ProductServiceTest()
        {
            _uow = Substitute.For<IUnitOfWork>();
            _securityUtil = Substitute.For<ISecurityUtil>();

            _productService = new ProductService(_uow, _securityUtil);
        }

        [Fact]
        public async Task AddProductAsync_WithValidData_ReturnsServiceResultWithData()
        {
            // Arrange


            var form = new ProductForm
            {
                ProductName = "Test Product",
                ProductDescription = "Test Product Description",
                CategoryId = 1,
                Price = 10.00,
                Location = new LocationForm
                {
                    Address = "Test Address",
                    Latitude = 10.00,
                    Longitude = 20.00
                }
            };
            var userId = 1;

            var category = new Category()
            {
                CategoryId = 1,
                CategoryName = "Test"
            };

            // Mock the CategoryRepository's FindByIdAndStatusAsync method to return a valid category
            _uow.CategoryRepository.FindByIdAndStatusAsync(Arg.Any<int>(), Arg.Any<Category.CategoryStatus>())
                .Returns(new Category());

            // Mock the ProductRepository's AddAsync method to return a new product
            var newProduct = new Product
            {
                ProductName = form.ProductName.Trim(),
                ProductDescription = form.ProductDescription?.Trim(),
                CategoryId = form.CategoryId,
                CreatedUserId = userId,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                Price = form.Price,
                Address = form.Location.Address,
                Latitude = form.Location.Latitude,
                Longitude = form.Location.Longitude,
                Status = Product.ProductStatus.DRAFT,
                Category = category,
            };
            _uow.ProductRepostory.Add(Arg.Any<Product>()).Returns(newProduct);

            // Act
            var result = await _productService.AddProductAsync(form, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.NotNull(result.Data);
            var productView = result.Data as ProductView;
            Assert.NotNull(productView);
            Assert.Equal(newProduct.ProductId, productView.ProductId);
            Assert.Equal(newProduct.ProductName, productView.ProductName);
            Assert.Equal(newProduct.ProductDescription, productView.ProductDescription);
            Assert.Equal(newProduct.CategoryId, productView.CategoryId);
            Assert.Equal(newProduct.CreatedDate, productView.CreatedDate);
            Assert.Equal(newProduct.Price, productView.Price);
            Assert.Equal(newProduct.Address, productView.Address);
            Assert.Equal((byte)newProduct.Status, productView.Status);
            Assert.True(result.Status);
        }


        [Fact]
        public async Task AddProductAsync_WithInactiveCategory_ReturnsbadRequest()
        {
            // Arrange


            var form = new ProductForm
            {
                ProductName = "Test Product",
                ProductDescription = "Test Product Description",
                CategoryId = 1,
                Price = 10.00,
                Location = new LocationForm
                {
                    Address = "Test Address",
                    Latitude = 10.00,
                    Longitude = 20.00
                }
            };
            var userId = 1;

            // Mock the CategoryRepository's FindByIdAndStatusAsync method to return a valid category
            _uow.CategoryRepository.FindByIdAndStatusAsync(Arg.Any<int>(), Arg.Any<Category.CategoryStatus>())
                .ReturnsNull();

            // Act
            var result = await _productService.AddProductAsync(form, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Category Id", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task DeleteProductAsync_WhenProductDoesNotExist_ReturnsNotFoundResult()
        {
            // Arrange
            int productId = 1;
            int userId = 1;
            _uow.ProductRepostory.FindById(productId).ReturnsNull();

            // Act
            var result = await _productService.DeleteProductAsync(productId, userId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Product Not Found", result.Message);
            _uow.ProductRepostory.DidNotReceiveWithAnyArgs().Update(Arg.Any<Product>());
            await _uow.DidNotReceiveWithAnyArgs().SaveAsync();
            Assert.False(result.Status);
        }

        [Fact]
        public async Task DeleteProductAsync_WhenProductIsSold_ReturnsNotFoundResult()
        {
            // Arrange
            int productId = 1;
            int userId = 1;
            var product = new Product { ProductId = productId, Status = Product.ProductStatus.SOLD };
            _uow.ProductRepostory.FindById(productId).Returns(product);

            // Act
            var result = await _productService.DeleteProductAsync(productId, userId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Product Not Found", result.Message);
            _uow.ProductRepostory.DidNotReceiveWithAnyArgs().Update(Arg.Any<Product>());
            await _uow.DidNotReceiveWithAnyArgs().SaveAsync();
            Assert.False(result.Status);
        }

        [Fact]
        public async Task DeleteProductAsync_WhenProductIsDeleted_ReturnsNotFoundResult()
        {
            // Arrange
            int productId = 1;
            int userId = 1;
            var product = new Product { ProductId = productId, Status = Product.ProductStatus.DELETED };
            _uow.ProductRepostory.FindById(productId).Returns(product);

            // Act
            var result = await _productService.DeleteProductAsync(productId, userId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Product Not Found", result.Message);
            _uow.ProductRepostory.DidNotReceiveWithAnyArgs().Update(Arg.Any<Product>());
            await _uow.DidNotReceiveWithAnyArgs().SaveAsync();
            Assert.False(result.Status);
        }

        [Fact]
        public async Task DeleteProductAsync_WhenUserIsNotProductCreator_ReturnsUnauthorizedResult()
        {
            // Arrange
            int productId = 1;
            int userId = 1;
            var product = new Product { ProductId = productId, CreatedUserId = 2 };
            _uow.ProductRepostory.FindById(productId).Returns(product);

            // Act
            var result = await _productService.DeleteProductAsync(productId, userId);

            // Assert
            Assert.Equal(ServiceStatus.Unauthorized, result.ServiceStatus);
            Assert.Equal("Unauthorized Access", result.Message);
            _uow.ProductRepostory.DidNotReceiveWithAnyArgs().Update(Arg.Any<Product>());
            await _uow.DidNotReceiveWithAnyArgs().SaveAsync();
            Assert.False(result.Status);
        }

        [Fact]
        public async Task DeleteProductAsync_Success()
        {
            // Arrange
            int productId = 1;
            int userId = 2;

            var product = new Product
            {
                ProductId = productId,
                CreatedUserId = userId,
                Status = Product.ProductStatus.ACTIVE
            };

            _uow.ProductRepostory.FindById(productId).Returns(product);
            _securityUtil.GetCurrentUserId().Returns(userId);

            var service = new ProductService(_uow, _securityUtil);

            // Act
            var result = await service.DeleteProductAsync(productId, userId);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Product deleted", result.Message);
            Assert.Equal(Product.ProductStatus.DELETED, product.Status);
            _uow.ProductRepostory.Received().Update(Arg.Is<Product>(p => p == product));
            await _uow.Received().SaveAsync();
            Assert.True(result.Status);
        }

        [Fact]
        public async Task EditProductAsync_WithValidData_ReturnsSuccessResult()
        {
            // Arrange
            var form = new ProductForm
            {
                ProductName = "Test Product",
                ProductDescription = "Test Product Description",
                CategoryId = 1,
                Price = 10.00,
                Location = new LocationForm
                {
                    Address = "Test Address",
                    Latitude = 10.00,
                    Longitude = 20.00
                }
            };
            var productId = 1;
            var userId = 2;

            var user = new User()
            {
                UserId = userId,
                Status = User.UserStatus.ACTIVE
            };

            var category = new Category
            {
                CategoryId = 1,
                CategoryName = "Test",
                Status = Category.CategoryStatus.ACTIVE
            };

            var photo = new Photos
            {
                PhotosId = 1,
                ProductId = productId,
                Photo = "productimage.png"
            };
            var photos = new List<Photos> { photo };

            var product = new Product
            {
                ProductId = productId,
                CreatedUserId = userId,
                CreatedUser = user,
                ProductName = "Test",
                ProductDescription = "Test",
                Price = 1000.00,
                Category = category,
                CategoryId = 1,
                Address = "address",
                CreatedDate = DateTime.Now,
                Latitude = 60.00,
                Longitude = 70.00,
                Photos = photos,
                Status = Product.ProductStatus.ACTIVE,
                UpdatedDate = DateTime.Now
            };

            _uow.ProductRepostory.FindById(productId).Returns(product);
            _uow.CategoryRepository.FindById(form.CategoryId).Returns(category);
            _uow.PhotoRepository.FindByProductIdAsync(productId).Returns(photos);
            _uow.ProductRepostory.Update(product).Returns(product);

            // Act
            var result = await _productService.EditProductAsync(form, productId, userId);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.NotNull(result.Data);
            Assert.IsType<ProductDetailView>(result.Data);
            var productDetailView = (ProductDetailView)result.Data;
            Assert.Equal(product.ProductId, productDetailView.ProductId);
            Assert.Equal(product.ProductName, productDetailView.ProductName);
            Assert.Equal(product.CategoryId, productDetailView.CategoryId);
            Assert.Equal(product.Category.CategoryName, productDetailView.CategoryName);
            Assert.Equal(product.ProductDescription, productDetailView.ProductDescription);
            Assert.Equal(product.Address, productDetailView.Address);
            Assert.Equal(product.Price, productDetailView.Price);
            Assert.Equal(product.CreatedDate, productDetailView.CreatedDate);
            Assert.Equal((byte)product.Status, productDetailView.Status);

            Assert.True(result.Status);
        }

        [Fact]
        public async Task EditProductAsync_ProductIsNull_ReturnsNotFound()
        {
            // Arrange
            var form = new ProductForm
            {
                ProductName = "Test Product",
                ProductDescription = "Test Product Description",
                CategoryId = 1,
                Price = 10.00,
                Location = new LocationForm
                {
                    Address = "Test Address",
                    Latitude = 10.00,
                    Longitude = 20.00
                }
            };
            var productId = 1;
            var userId = 2;


            _uow.ProductRepostory.FindById(productId).ReturnsNull();

            // Act
            var result = await _productService.EditProductAsync(form, productId, userId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Product Not Found", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task EditProductAsync_CreatedUserIDifferent_ReturnsNotFound()
        {
            // Arrange
            var form = new ProductForm
            {
                ProductName = "Test Product",
                ProductDescription = "Test Product Description",
                CategoryId = 1,
                Price = 10.00,
                Location = new LocationForm
                {
                    Address = "Test Address",
                    Latitude = 10.00,
                    Longitude = 20.00
                }
            };
            var productId = 1;
            var userId = 2;

            var user = new User()
            {
                UserId = 123,
                Status = User.UserStatus.ACTIVE
            };

            var category = new Category
            {
                CategoryId = 1,
                CategoryName = "Test",
                Status = Category.CategoryStatus.ACTIVE
            };

            var photo = new Photos
            {
                PhotosId = 1,
                ProductId = productId,
                Photo = "productimage.png"
            };
            var photos = new List<Photos> { photo };

            var product = new Product
            {
                ProductId = productId,
                CreatedUserId = 123,
                CreatedUser = user,
                ProductName = "Test",
                ProductDescription = "Test",
                Price = 1000.00,
                Category = category,
                CategoryId = 1,
                Address = "address",
                CreatedDate = DateTime.Now,
                Latitude = 60.00,
                Longitude = 70.00,
                Photos = photos,
                Status = Product.ProductStatus.ACTIVE,
                UpdatedDate = DateTime.Now
            };

            _uow.ProductRepostory.FindById(productId).Returns(product);

            // Act
            var result = await _productService.EditProductAsync(form, productId, userId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Product Not Found", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task EditProductAsync_DeletedProduct_ReturnsNotFound()
        {
            // Arrange
            var form = new ProductForm
            {
                ProductName = "Test Product",
                ProductDescription = "Test Product Description",
                CategoryId = 1,
                Price = 10.00,
                Location = new LocationForm
                {
                    Address = "Test Address",
                    Latitude = 10.00,
                    Longitude = 20.00
                }
            };
            var productId = 1;
            var userId = 2;

            var user = new User()
            {
                UserId = 123,
                Status = User.UserStatus.ACTIVE
            };

            var category = new Category
            {
                CategoryId = 1,
                CategoryName = "Test",
                Status = Category.CategoryStatus.ACTIVE
            };

            var photo = new Photos
            {
                PhotosId = 1,
                ProductId = productId,
                Photo = "productimage.png"
            };
            var photos = new List<Photos> { photo };

            var product = new Product
            {
                ProductId = productId,
                CreatedUserId = 123,
                CreatedUser = user,
                ProductName = "Test",
                ProductDescription = "Test",
                Price = 1000.00,
                Category = category,
                CategoryId = 1,
                Address = "address",
                CreatedDate = DateTime.Now,
                Latitude = 60.00,
                Longitude = 70.00,
                Photos = photos,
                Status = Product.ProductStatus.DELETED,
                UpdatedDate = DateTime.Now
            };

            _uow.ProductRepostory.FindById(productId).Returns(product);

            // Act
            var result = await _productService.EditProductAsync(form, productId, userId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Product Not Found", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task EditProductAsync_SoldProduct_ReturnsNotFound()
        {
            // Arrange
            var form = new ProductForm
            {
                ProductName = "Test Product",
                ProductDescription = "Test Product Description",
                CategoryId = 1,
                Price = 10.00,
                Location = new LocationForm
                {
                    Address = "Test Address",
                    Latitude = 10.00,
                    Longitude = 20.00
                }
            };
            var productId = 1;
            var userId = 2;

            var user = new User()
            {
                UserId = 123,
                Status = User.UserStatus.ACTIVE
            };

            var category = new Category
            {
                CategoryId = 1,
                CategoryName = "Test",
                Status = Category.CategoryStatus.ACTIVE
            };

            var photo = new Photos
            {
                PhotosId = 1,
                ProductId = productId,
                Photo = "productimage.png"
            };
            var photos = new List<Photos> { photo };

            var product = new Product
            {
                ProductId = productId,
                CreatedUserId = 123,
                CreatedUser = user,
                ProductName = "Test",
                ProductDescription = "Test",
                Price = 1000.00,
                Category = category,
                CategoryId = 1,
                Address = "address",
                CreatedDate = DateTime.Now,
                Latitude = 60.00,
                Longitude = 70.00,
                Photos = photos,
                Status = Product.ProductStatus.SOLD,
                UpdatedDate = DateTime.Now
            };

            _uow.ProductRepostory.FindById(productId).Returns(product);

            // Act
            var result = await _productService.EditProductAsync(form, productId, userId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Product Not Found", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task EditProductAsync_CategoryNull_ReturnsBadrequest()
        {
            // Arrange
            var form = new ProductForm
            {
                ProductName = "Test Product",
                ProductDescription = "Test Product Description",
                CategoryId = 1,
                Price = 10.00,
                Location = new LocationForm
                {
                    Address = "Test Address",
                    Latitude = 10.00,
                    Longitude = 20.00
                }
            };
            var productId = 1;
            var userId = 2;

            var user = new User()
            {
                UserId = userId,
                Status = User.UserStatus.ACTIVE
            };

            var category = new Category
            {
                CategoryId = 1,
                CategoryName = "Test",
                Status = Category.CategoryStatus.INACTIVE
            };

            var photo = new Photos
            {
                PhotosId = 1,
                ProductId = productId,
                Photo = "productimage.png"
            };
            var photos = new List<Photos> { photo };

            var product = new Product
            {
                ProductId = productId,
                CreatedUserId = userId,
                CreatedUser = user,
                ProductName = "Test",
                ProductDescription = "Test",
                Price = 1000.00,
                Category = category,
                CategoryId = 1,
                Address = "address",
                CreatedDate = DateTime.Now,
                Latitude = 60.00,
                Longitude = 70.00,
                Photos = photos,
                Status = Product.ProductStatus.ACTIVE,
                UpdatedDate = DateTime.Now
            };

            _uow.ProductRepostory.FindById(productId).Returns(product);
            _uow.CategoryRepository.FindById(form.CategoryId).ReturnsNull();

            // Act
            var result = await _productService.EditProductAsync(form, productId, userId);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Category Id", result.Message);
            Assert.False(result.Status);

        }

        [Fact]
        public async Task EditProductAsync_PhotosNull_ReturnsSuccessResult()
        {
            // Arrange
            var form = new ProductForm
            {
                ProductName = "Test Product",
                CategoryId = 1,
                Price = 10.00,
                Location = new LocationForm
                {
                    Address = "Test Address",
                    Latitude = 10.00,
                    Longitude = 20.00
                }
            };
            var productId = 1;
            var userId = 2;

            var user = new User()
            {
                UserId = userId,
                Status = User.UserStatus.ACTIVE
            };

            var category = new Category
            {
                CategoryId = 1,
                CategoryName = "Test",
                Status = Category.CategoryStatus.ACTIVE
            };


            var photos = new List<Photos>() { };

            var product = new Product
            {
                ProductId = productId,
                CreatedUserId = userId,
                CreatedUser = user,
                ProductName = "Test",
                ProductDescription = "Test",
                Price = 1000.00,
                Category = category,
                CategoryId = 1,
                Address = "address",
                CreatedDate = DateTime.Now,
                Latitude = 60.00,
                Longitude = 70.00,
                Photos = photos,
                Status = Product.ProductStatus.ACTIVE,
                UpdatedDate = DateTime.Now
            };

            _uow.ProductRepostory.FindById(productId).Returns(product);
            _uow.CategoryRepository.FindById(form.CategoryId).Returns(category);
            _uow.PhotoRepository.FindByProductIdAsync(productId).Returns(photos);
            _uow.ProductRepostory.Update(product).Returns(product);

            // Act
            var result = await _productService.EditProductAsync(form, productId, userId);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.NotNull(result.Data);
            Assert.IsType<ProductDetailView>(result.Data);
            var productDetailView = (ProductDetailView)result.Data;
            Assert.Equal(product.ProductId, productDetailView.ProductId);
            Assert.Equal(product.ProductName, productDetailView.ProductName);
            Assert.Equal(product.CategoryId, productDetailView.CategoryId);
            Assert.Equal(product.Category.CategoryName, productDetailView.CategoryName);
            Assert.Equal(product.ProductDescription, productDetailView.ProductDescription);
            Assert.Equal(product.Address, productDetailView.Address);
            Assert.Equal(product.Price, productDetailView.Price);
            Assert.Equal(product.CreatedDate, productDetailView.CreatedDate);
            Assert.Equal((byte)product.Status, productDetailView.Status);
            Assert.True(result.Status);
            Assert.NotNull(productDetailView.CreatedUser);
            Assert.NotNull(productDetailView.Location);
            Assert.Equal(product.CreatedUser.UserId, productDetailView.CreatedUser.UserId);
            Assert.Equal(product.CreatedUser.FirstName, productDetailView.CreatedUser.FirstName);
            Assert.Equal(product.CreatedUser.LastName, productDetailView.CreatedUser.LastName);
            Assert.Equal(product.CreatedUser.Email, productDetailView.CreatedUser.Email);
            Assert.Equal((byte)product.CreatedUser.Role, productDetailView.CreatedUser.Role);
            Assert.Equal((byte)product.CreatedUser.Status, productDetailView.CreatedUser.Status);
            Assert.Equal(product.CreatedUser.CreatedDate, productDetailView.CreatedUser.CreatedDate);
            Assert.Equal(product.CreatedUser.ProfilePic, productDetailView.CreatedUser.ProfilePic);
            Assert.Equal(product.Latitude, productDetailView.Location.Latitude);
            Assert.Equal(product.Longitude, productDetailView.Location.Longitude);

        }

        [Fact]
        public async Task GetProductAsync_Returns_ProductDetailView_When_Product_Is_Active_And_User_Has_Permission()
        {
            // Arrange
            int productId = 1;
            int userId = 2;

            var user = new User()
            {
                UserId = userId,
                Status = User.UserStatus.ACTIVE
            };

            var category = new Category()
            {
                CategoryId = 1,
                CategoryName = "Test",
            };

            var product = new Product
            {
                ProductId = productId,
                CreatedUserId = 123,
                Status = Product.ProductStatus.ACTIVE,
                CreatedUser = user,
                Category = category,
            };
            var photo = new Photos { Photo = "ProductImage.Jpg" };
            product.Photos = new List<Photos> { photo };
            _uow.ProductRepostory.FindById(productId).Returns(product);

            // Act
            var result = await _productService.GetProductAsync(productId, userId);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.IsType<ProductDetailView>(result.Data);
            var productDetailView = (ProductDetailView)result.Data;
            Assert.Equal(product.ProductId, productDetailView.ProductId);
            Assert.Equal(product.ProductName, productDetailView.ProductName);
            Assert.Equal(product.CategoryId, productDetailView.CategoryId);
            Assert.Equal(product.Category.CategoryName, productDetailView.CategoryName);
            Assert.Equal(product.ProductDescription, productDetailView.ProductDescription);
            Assert.Equal(product.Address, productDetailView.Address);
            Assert.Equal(product.Price, productDetailView.Price);
            Assert.Equal(product.CreatedDate, productDetailView.CreatedDate);
            Assert.Equal((byte)product.Status, productDetailView.Status);
            Assert.True(result.Status);
            Assert.NotNull(productDetailView.CreatedUser);
            Assert.NotNull(productDetailView.Location);
            Assert.Equal(product.CreatedUser.UserId, productDetailView.CreatedUser.UserId);
            Assert.Equal(product.CreatedUser.FirstName, productDetailView.CreatedUser.FirstName);
            Assert.Equal(product.CreatedUser.LastName, productDetailView.CreatedUser.LastName);
            Assert.Equal(product.CreatedUser.Email, productDetailView.CreatedUser.Email);
            Assert.Equal((byte)product.CreatedUser.Role, productDetailView.CreatedUser.Role);
            Assert.Equal((byte)product.CreatedUser.Status, productDetailView.CreatedUser.Status);
            Assert.Equal(product.CreatedUser.CreatedDate, productDetailView.CreatedUser.CreatedDate);
            Assert.Equal(product.CreatedUser.ProfilePic, productDetailView.CreatedUser.ProfilePic);
            Assert.Equal(product.Latitude, productDetailView.Location.Latitude);
            Assert.Equal(product.Longitude, productDetailView.Location.Longitude);
        }

        [Fact]
        public async Task GetProductAsync_CreatedUserIsNotActive_ReturnsNotFound()
        {
            // Arrange
            int productId = 1;
            int userId = 2;

            var user = new User()
            {
                UserId = userId,
                Status = User.UserStatus.INACTIVE
            };

            var category = new Category()
            {
                CategoryId = 1,
                CategoryName = "Test",
            };

            var product = new Product
            {
                ProductId = productId,
                CreatedUserId = 123,
                Status = Product.ProductStatus.ACTIVE,
                CreatedUser = user,
                Category = category,
            };
            var photo = new Photos { Photo = "ProductImage.Jpg" };
            product.Photos = new List<Photos> { photo };
            _uow.ProductRepostory.FindById(productId).Returns(product);

            // Act
            var result = await _productService.GetProductAsync(productId, userId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Product Not Found", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task GetProductAsync_PublicUserAndNotactiveProduct_Returns()
        {
            // Arrange
            int productId = 1;
            int userId = 2;

            var user = new User()
            {
                UserId = userId,
                Status = User.UserStatus.ACTIVE
            };

            var category = new Category()
            {
                CategoryId = 1,
                CategoryName = "Test",
            };

            var product = new Product
            {
                ProductId = productId,
                CreatedUserId = 123,
                Status = Product.ProductStatus.INACTIVE,
                CreatedUser = user,
                Category = category,
            };
            var photo = new Photos { Photo = "ProductImage.Jpg" };
            product.Photos = new List<Photos> { photo };
            _uow.ProductRepostory.FindById(productId).Returns(product);
            _securityUtil.GetCurrentUserId().Returns(0);

            // Act
            var result = await _productService.GetProductAsync(productId, userId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Product Not Found", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task GetProductAsync_ProductIsNull_ReturnsNotFound()
        {
            // Arrange
            int productId = 1;
            int userId = 2;

            _uow.ProductRepostory.FindById(productId).ReturnsNull();

            // Act
            var result = await _productService.GetProductAsync(productId, userId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Product Not Found", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task GetProductAsync_PRoductIsSold_Returns()
        {
            // Arrange
            int productId = 1;
            int userId = 2;

            var user = new User()
            {
                UserId = userId,
                Status = User.UserStatus.ACTIVE
            };

            var category = new Category()
            {
                CategoryId = 1,
                CategoryName = "Test",
            };

            var product = new Product
            {
                ProductId = productId,
                CreatedUserId = 123,
                Status = Product.ProductStatus.SOLD,
                CreatedUser = user,
                Category = category,
            };
            var photo = new Photos { Photo = "ProductImage.Jpg" };
            product.Photos = new List<Photos> { photo };
            _uow.ProductRepostory.FindById(productId).Returns(product);
            _uow.OrderDetailsRepository.FindByBuyerIdAndProductId(userId, productId).Returns(Array.Empty<OrderDetails>().ToList());
            _securityUtil.GetCurrentUserId().Returns(0);

            // Act
            var result = await _productService.GetProductAsync(productId, userId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Product not found", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task GetProductAsync_PRoductIsSold_ReturnsSuccess()
        {
            // Arrange
            int productId = 1;
            int userId = 2;

            var user = new User()
            {
                UserId = userId,
                Status = User.UserStatus.ACTIVE
            };

            var category = new Category()
            {
                CategoryId = 1,
                CategoryName = "Test",
            };

            var product = new Product
            {
                ProductId = productId,
                CreatedUserId = 123,
                Status = Product.ProductStatus.SOLD,
                CreatedUser = user,
                Category = category,
            };
            var photo = new Photos { Photo = "ProductImage.Jpg" };
            product.Photos = new List<Photos> { photo };
            _uow.ProductRepostory.FindById(productId).Returns(product);
            _uow.OrderDetailsRepository.FindByBuyerIdAndProductId(userId, productId).Returns(new List<OrderDetails>() { new OrderDetails() { ProductId = productId } });
            _securityUtil.GetCurrentUserId().Returns(0);

            // Act
            var result = await _productService.GetProductAsync(productId, userId);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.True(result.Status);
        }

        //[Fact]
        //public async Task ProductListAsync_ReturnsSuccessWithPagerOffset_WhenGivenValidParameters()
        //{
        //    // Arrange
        //    var form = new ProductPaginationParams
        //    {
        //        Search = "test",
        //        CategoryId = new int?[] {1,2},
        //        StartPrice = 10,
        //        EndPrice = 100,
        //        SortBy = "CreatedDate",
        //        SortByDesc = false,
        //        Location = "test"
        //    };
        //    var role = User.UserRole.USER;
        //    var productList = new List<Product>
        //{
        //    new Product { ProductId = 1, ProductName = "Test Product 1", Price = 50, CreatedUser = new User { Status = 1 }, Photos = new List<Photos> { new Photos { Photo = "test.png" } } },
        //    new Product { ProductId = 2, ProductName = "Test Product 2", Price = 75, CreatedUser = new User { Status = 1 }, Photos = new List<Photos> { new Photos { Photo = "test2.png" } } },
        //    new Product { ProductId = 3, ProductName = "Test Product 3", Price = 90, CreatedUser = new User { Status = 1 }, Photos = new List<Photos> { new Photos { Photo = "test3.png" } } },
        //};
        //    _uow.ProductRepostory.FindAllByCategoryOrBrandLikeAndPriceAndLocationBetweenAsync(
        //        Arg.Any<int[]>(), Arg.Any<float>(), Arg.Any<float>(), Arg.Any<bool>(), Arg.Any<byte[]>(), Arg.Any<Dictionary<string, string?>>())
        //        .Returns(productList);
        //    _securityUtil.GetCurrentUserId().Returns(1);

        //    // Act
        //    var result = await _productService.ProductListAsync(form, role);

        //    // Assert
        //    Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
        //    Assert.IsType<PagerOffset<ProductView>>(result.Data);

        //}

        [Fact]
        public async Task GetProductByUserIdAsync_Returns_Products_For_Valid_UserId()
        {
            // Arrange
            var userId = 1;
            var products = new List<Product>
        {
            new Product { ProductId = 1, ProductName = "Product 1", Category = new Category(){CategoryId = 1, CategoryName = "Test"}, ProductDescription = "Description 1", Status = Product.ProductStatus.ACTIVE },
            new Product { ProductId = 2, ProductName = "Product 2", Category = new Category(){CategoryId = 1, CategoryName = "Test2"}, ProductDescription = "Description 2", Status = Product.ProductStatus.ACTIVE }
        };
            _uow.ProductRepostory.FindByUserIAsync(userId).Returns(products);
            _securityUtil.GetCurrentUserId().Returns(1);
            var productService = new ProductService(_uow, _securityUtil);

            // Act
            var result = await productService.GetProductByUserIdAsync(userId);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.NotNull(result.Data);
            Assert.Equal(2, ((List<ProductView>)result.Data).Count);
            Assert.True(result.Status);
        }

        [Fact]
        public async Task GetProductByUserIdAsync_Returns_NotFound_For_Invalid_UserId()
        {
            // Arrange
            var userId = 1;
            var unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.ProductRepostory.FindByUserIAsync(userId).Returns(new List<Product>());
            var securityUtil = Substitute.For<ISecurityUtil>();
            securityUtil.GetCurrentUserId().Returns(1);
            var productService = new ProductService(unitOfWork, securityUtil);

            // Act
            var result = await productService.GetProductByUserIdAsync(userId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("No Products found", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task GetProductByUserIdAsync_Filters_Deleted_Products_For_Current_User()
        {
            // Arrange
            var userId = 1;
            var products = new List<Product>
            {
                new Product { ProductId = 1, ProductName = "Product 1", Category = new Category(){CategoryId = 1, CategoryName = "Test"}, ProductDescription = "Description 1", Status = Product.ProductStatus.ACTIVE },
                new Product { ProductId = 2, ProductName = "Product 2", Category = new Category(){CategoryId = 1, CategoryName = "Test"}, ProductDescription = "Description 2", Status = Product.ProductStatus.DELETED }
            };
            var unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.ProductRepostory.FindByUserIAsync(userId).Returns(products);
            var securityUtil = Substitute.For<ISecurityUtil>();
            securityUtil.GetCurrentUserId().Returns(userId);
            var productService = new ProductService(unitOfWork, securityUtil);

            // Act
            var result = await productService.GetProductByUserIdAsync(userId);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.NotNull(result.Data);
            Assert.Single((List<ProductView>)result.Data);
            Assert.Equal("Product 1", ((List<ProductView>)result.Data)[0].ProductName);
            Assert.True(result.Status);
        }

        [Fact]
        public async Task GetProductByUserIdAsync_Returns_Products_For_Valid_UserId_isnot_CurrentUser()
        {
            // Arrange
            var userId = 1;
            var products = new List<Product>
        {
            new Product { ProductId = 1, ProductName = "Product 1", Category = new Category(){CategoryId = 1, CategoryName = "Test"}, ProductDescription = "Description 1", Status = Product.ProductStatus.ACTIVE },
            new Product { ProductId = 2, ProductName = "Product 2", Category = new Category(){CategoryId = 1, CategoryName = "Test2"}, ProductDescription = "Description 2", Status = Product.ProductStatus.ACTIVE }
        };
            _uow.ProductRepostory.FindByUserIAsync(userId).Returns(products);
            _securityUtil.GetCurrentUserId().Returns(2);
            var productService = new ProductService(_uow, _securityUtil);

            // Act
            var result = await productService.GetProductByUserIdAsync(userId);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.NotNull(result.Data);
            Assert.Equal(2, ((List<ProductView>)result.Data).Count);
            Assert.True(result.Status);
        }


        [Fact]
        public async Task ChangeStatusAsync_WhenProductNotSold_ShouldUpdateProductStatusAndSaveChanges()
        {
            // Arrange
            var productService = new ProductService(_uow, _securityUtil);
            var product = new Product
            {
                Status = Product.ProductStatus.ACTIVE
            };
            var newStatus = Product.ProductStatus.SOLD;

            // Act
            await productService.ChangeStatusAsync(product, newStatus);

            // Assert
            Assert.Equal(newStatus, product.Status);
            _uow.ProductRepostory.Received(1).Update(product);
            await _uow.Received(1).SaveAsync();
        }

        [Fact]
        public async Task ChangeStatusAsync_UpdatesProductStatus()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockProductRepository = new Mock<IProductRepostory>();
            var product = new Product { Status = Product.ProductStatus.ACTIVE };
            mockProductRepository.Setup(x => x.Update(It.IsAny<Product>()));
            mockUnitOfWork.SetupGet(x => x.ProductRepostory).Returns(mockProductRepository.Object);
            var service = new ProductService(mockUnitOfWork.Object, _securityUtil);

            // Act
            await service.ChangeStatusAsync(product, Product.ProductStatus.PENDING);

            // Assert
            Assert.Equal(Product.ProductStatus.PENDING, product.Status);
        }

        [Fact]
        public async Task ChangeStatusAsync_DoesNotUpdateProductStatusWhenSold()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockProductRepository = new Mock<IProductRepostory>();
            var product = new Product { Status = Product.ProductStatus.SOLD };
            mockUnitOfWork.SetupGet(x => x.ProductRepostory).Returns(mockProductRepository.Object);
            var service = new ProductService(mockUnitOfWork.Object, _securityUtil);

            // Act
            await service.ChangeStatusAsync(product, Product.ProductStatus.PENDING);

            // Assert
            mockProductRepository.Verify(x => x.Update(It.IsAny<Product>()), Times.Never);
            mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Never);
        }

        [Fact]
        public async Task ChangeStatusAsync_SetsUpdatedDate()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockProductRepository = new Mock<IProductRepostory>();
            var product = new Product { Status = Product.ProductStatus.ACTIVE };
            mockProductRepository.Setup(x => x.Update(It.IsAny<Product>()));
            mockUnitOfWork.SetupGet(x => x.ProductRepostory).Returns(mockProductRepository.Object);
            var service = new ProductService(mockUnitOfWork.Object, _securityUtil);

            // Act
            await service.ChangeStatusAsync(product, Product.ProductStatus.PENDING);

            // Assert
            Assert.True(product.UpdatedDate > DateTime.MinValue);
        }
    }
}
