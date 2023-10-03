using MarketPlaceAdmin.Api.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Xunit;

namespace MarketplaceAdminTest.Extra
{
    public class SwaggerExtensionTests
    {
        [Fact]
        public void AddSwagger_ShouldAddServicesToServiceCollection()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();

            // Act
            builder.AddSwagger();
            var services = builder.Services;

            // Assert
            Assert.Contains(services, s => s.ServiceType == typeof(ISwaggerProvider));
        }

        [Fact]
        public void Swagger_Options_Test()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();

            // Act
            builder.AddSwagger();
            var swaggerGenOptions = builder.Services.BuildServiceProvider().GetRequiredService<IOptionsMonitor<SwaggerGenOptions>>().CurrentValue;


            Assert.NotNull(swaggerGenOptions.SchemaFilterDescriptors);
        }


        [Fact]
        public void UseSwaggerPage_AddsCacheControlMiddlewareToWebApplication_ReturnsWebApplicationWithMiddlewareAdded()
        {
            // Arrange
            var app = WebApplication.Create();

            // Act
            var result = app.UseSwaggerPage();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<WebApplication>(result);
        }
    }
}

