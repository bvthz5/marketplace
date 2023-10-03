using MarketPlaceAdmin.Api.Middlewares;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace MarketplaceAdminTest.Middlewares
{
    public class CacheControlMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_SetsCacheControlHeader()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var response = httpContext.Response;
            var middleware = new CacheControlMiddleware();

            // Act
            await middleware.InvokeAsync(httpContext, context => Task.CompletedTask);

            // Assert
            Assert.Equal("no-cache, no-store, must-revalidate", response.Headers["Cache-Control"]);
        }
    }

}
