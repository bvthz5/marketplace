using MarketPlace.DataAccess.Interfaces;
using MarketPlaceAdmin.Api.Extensions;
using MarketPlaceAdmin.Bussiness.Interfaces;
using MarketPlaceAdmin.Bussiness.Security.Interfaces;
using MarketPlaceAdmin.Bussiness.Settings;
using MarketPlaceAdmin.Bussiness.Util.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Xunit;

namespace MarketplaceAdminTest.Extra
{
    public class ConfigureSettingsTests
    {
        [Fact]
        public void ConfigureGoogleAuthSettings_Succeeds()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            builder.Configuration.AddJsonFile("appsettings.json");

            // Act
            builder.ConfigureSettings();

            // Assert
            var serviceProvider = builder.Services.BuildServiceProvider();
            var settingsOptions = serviceProvider.GetService<IOptions<GoogleAuthSettings>>();
            Assert.NotNull(settingsOptions);
            var settings = settingsOptions.Value;
            Assert.NotNull(settings);
            Assert.Equal("436868628549-2i44ia9b40k6ev6s292gqrsld523d7jb.apps.googleusercontent.com", settings.ClientId);
            Assert.Equal("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", settings.ClientSecret);


        }

        [Fact]
        public void ConfigureMailSettings_Succeeds()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            builder.Configuration.AddJsonFile("appsettings.json");

            // Act
            builder.ConfigureSettings();

            // Assert
            var services = builder.Services.BuildServiceProvider();
            var settingsOptions = services.GetService<IOptions<MailSettings>>();
            Assert.NotNull(settingsOptions);
            var settings = settingsOptions.Value;
            Assert.NotNull(settings);
            Assert.Equal("smtp.gmail.com", settings.Host);
            Assert.Equal(587, settings.Port);
            Assert.Equal("Market Place", settings.DisplayName);
            Assert.NotEmpty(settings.Pswd);
            Assert.Equal("invmarketplace4u@gmail.com", settings.Mail);

        }

        [Fact]
        public void ConfigureJwtSettings_Succeeds()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            builder.Configuration.AddJsonFile("appsettings.json");

            // Act
            builder.ConfigureSettings();

            // Assert
            var services = builder.Services.BuildServiceProvider();
            var settingsOptions = services.GetService<IOptions<JwtSettings>>();
            Assert.NotNull(settingsOptions);
            var settings = settingsOptions.Value;
            Assert.NotNull(settings);

            Assert.NotNull(settings);
            Assert.Equal("Admin-ACDt1vR3lXToPQ1g3MyN", settings.Key);
            Assert.Equal("https://stg-api-admin-marketplacebe.innovaturelabs.com/", settings.Issuer);
            Assert.Equal("https://stg-admin-marketplacefe.innovaturelabs.com/", settings.Audience);
            Assert.Equal(240, settings.AccessTokenExpiry);
            Assert.Equal(10080, settings.RefreshTokenExpiry);

        }

        [Fact]
        public void ConfigureImageSettings_Succeeds()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            builder.Configuration.AddJsonFile("appsettings.json");

            // Act
            builder.ConfigureSettings();

            // Assert
            var services = builder.Services.BuildServiceProvider();
            var settingsOptions = services.GetService<IOptions<ImageSettings>>();
            Assert.NotNull(settingsOptions);
            var settings = settingsOptions.Value;
            Assert.NotNull(settings);
            Assert.NotNull(settings);
            Assert.Equal("D://MarketplaceProduction_Phase3/Resources", settings.Path);
            Assert.Equal("ProductImages", settings.ProductImagePath);
            Assert.Equal("UserProfilePic", settings.UserImagePath);

        }

        [Fact]
        public void AddJwt_ConfiguresJwtBearer()
        {
            // Arrange
            var builder = new HostBuilder().ConfigureAppConfiguration(config =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"JwtSettings:Issuer", "testissuer"},
                    {"JwtSettings:Audience", "testaudience"},
                    {"JwtSettings:Key", "testkey"}
                });
            }).ConfigureServices(services =>
            {
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "testissuer",
                        ValidAudience = "testaudience",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("testkey"))
                    };
                });
            });

            // Act
            var host = builder.Build();

            // Assert
            var serviceProvider = host.Services;
            var options = serviceProvider.GetService<IOptionsMonitor<JwtBearerOptions>>();
            var bearerOptions = options?.Get(JwtBearerDefaults.AuthenticationScheme);
            Assert.NotNull(bearerOptions);
            Assert.Equal("testissuer", bearerOptions.TokenValidationParameters.ValidIssuer);
            Assert.Equal("testaudience", bearerOptions.TokenValidationParameters.ValidAudience);
            Assert.Equal("testkey", Encoding.UTF8.GetString(((SymmetricSecurityKey)bearerOptions.TokenValidationParameters.IssuerSigningKey!).Key));


        }


        [Fact]
        public void Registered_with_the_correct_lifetime()
        {
            var builder = WebApplication.CreateBuilder();
            builder.RegisterServices();

            builder.Services.BuildServiceProvider();
            Assert.Equal(ServiceLifetime.Scoped, builder.Services.FirstOrDefault(x => x.ServiceType == typeof(IUnitOfWork))?.Lifetime);
            Assert.Equal(ServiceLifetime.Scoped, builder.Services.FirstOrDefault(x => x.ServiceType == typeof(IAdminService))?.Lifetime);
            Assert.Equal(ServiceLifetime.Scoped, builder.Services.FirstOrDefault(x => x.ServiceType == typeof(IDashboardService))?.Lifetime);
            Assert.Equal(ServiceLifetime.Scoped, builder.Services.FirstOrDefault(x => x.ServiceType == typeof(IUserService))?.Lifetime);
            Assert.Equal(ServiceLifetime.Scoped, builder.Services.FirstOrDefault(x => x.ServiceType == typeof(ICategoryService))?.Lifetime);
            Assert.Equal(ServiceLifetime.Scoped, builder.Services.FirstOrDefault(x => x.ServiceType == typeof(IPhotosService))?.Lifetime);
            Assert.Equal(ServiceLifetime.Scoped, builder.Services.FirstOrDefault(x => x.ServiceType == typeof(IProductService))?.Lifetime);
            Assert.Equal(ServiceLifetime.Scoped, builder.Services.FirstOrDefault(x => x.ServiceType == typeof(IGoogleAuthService))?.Lifetime);
            Assert.Equal(ServiceLifetime.Scoped, builder.Services.FirstOrDefault(x => x.ServiceType == typeof(IEmailService))?.Lifetime);
            Assert.Equal(ServiceLifetime.Scoped, builder.Services.FirstOrDefault(x => x.ServiceType == typeof(IAdminOrderService))?.Lifetime);
            Assert.Equal(ServiceLifetime.Scoped, builder.Services.FirstOrDefault(x => x.ServiceType == typeof(IAgentService))?.Lifetime);
            Assert.Equal(ServiceLifetime.Scoped, builder.Services.FirstOrDefault(x => x.ServiceType == typeof(ISecurityUtil))?.Lifetime);
            Assert.Equal(ServiceLifetime.Scoped, builder.Services.FirstOrDefault(x => x.ServiceType == typeof(IGoogleAuth))?.Lifetime);
            Assert.Equal(ServiceLifetime.Scoped, builder.Services.FirstOrDefault(x => x.ServiceType == typeof(ITokenGenerator))?.Lifetime);
            Assert.Equal(ServiceLifetime.Scoped, builder.Services.FirstOrDefault(x => x.ServiceType == typeof(IFileUtil))?.Lifetime);

        }

    }
}


