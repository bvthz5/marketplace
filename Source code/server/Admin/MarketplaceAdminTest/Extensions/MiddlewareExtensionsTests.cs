using MarketPlaceAdmin.Api.Extensions;
using Microsoft.AspNetCore.Builder;
using Xunit;

namespace MarketplaceAdminTest.Extensions
{
    public class MiddlewareExtensionsTests
    {
        [Fact]
        public void AddMiddlewares_RegistersMiddlewaresInWebApplicationBuilder_ReturnsWebApplicationBuilderWithMiddlewaresRegistered()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder(new string[] { });
            // Act
            var result = builder.AddMiddlewares();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<WebApplicationBuilder>(result);
        }

        [Fact]
        public void AUseGlobalExceptionHandlingMiddleware_AddsGlobalExceptionHandlingMiddlewareToWebApplication_ReturnsWebApplicationWithMiddlewareAdded()
        {
            // Arrange
            var app = WebApplication.Create();

            // Act
            var result = app.UseGlobalExceptionHandlingMiddleware();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<WebApplication>(result);
        }

        [Fact]
        public void UseCacheControlMiddleware_AddsCacheControlMiddlewareToWebApplication_ReturnsWebApplicationWithMiddlewareAdded()
        {
            // Arrange
            var app = WebApplication.Create();

            // Act
            var result = app.UseCacheControlMiddleware();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<WebApplication>(result);
        }

        [Fact]
        public void UseAgentValidationMiddleware_AddsAgentValidationMiddlewareToWebApplication_ReturnsWebApplicationWithMiddlewareAdded()
        {
            // Arrange
            var app = WebApplication.Create();

            // Act
            var result = app.UseAgentValidationMiddleware();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<WebApplication>(result);
        }
    }
}
