using MarketPlaceAdmin.Api.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;
using Xunit;

namespace MarketplaceAdminTest.Extensions
{
    public class LoggerExtensionTests
    {
        [Fact]
        public void AddCustomLogger_AddsSerilogLoggerToWebApplicationBuilder_ReturnsWebApplicationBuilderWithLoggerAdded()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            builder.Configuration.AddJsonFile("appsettings.json");

            // Act
            builder.AddCustomLogger();
            var result = builder.RegisterServices().Services.BuildServiceProvider();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ServiceProvider>(result);

            var loggerProvider = result.GetService<ILoggerProvider>();
            Assert.NotNull(loggerProvider);
            Assert.IsType<SerilogLoggerProvider>(loggerProvider);

            var logger = loggerProvider.CreateLogger("TestLogger");
            Assert.NotNull(logger);
        }
    }
}
