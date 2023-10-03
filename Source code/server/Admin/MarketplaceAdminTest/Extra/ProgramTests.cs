using MarketPlaceAdmin.Api.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;
using Xunit;


namespace MarketplaceAdminTest.Extra
{
    public class ProgramTests
    {
        [Fact]
        public void WebApplicationBuilder_instance_is_created_successfully()
        {
            var builder = WebApplication.CreateBuilder(new string[] { });
            Assert.NotNull(builder);

        }

        [Fact]
        public void Services_are_added_container()
        {
            var builder = WebApplication.CreateBuilder(new string[] { });
            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();
            builder.AddSqlConnection("connectionString");
            builder.RegisterServices();
            var serviceProvider = builder.Services.BuildServiceProvider();
            Assert.NotNull(serviceProvider.GetService<IHttpContextAccessor>());
        }



        [Fact]
        public void Swagger_documentation_web_application()
        {
            var builder = WebApplication.CreateBuilder(new string[] { });
            builder.AddSwagger();
            var serviceProvider = builder.Services.BuildServiceProvider();
            Assert.NotNull(serviceProvider.GetServices(typeof(SwaggerGenOptions)));

        }

        [Fact]
        public void JWT_authentication_web_application()
        {
            var builder = WebApplication.CreateBuilder(new string[] { });
            builder.AddJwt();
            var serviceProvider = builder.Services.BuildServiceProvider();
            Assert.NotNull(serviceProvider.GetService(typeof(IAuthenticationService)));

        }

        [Fact]
        public void JWT_Options_Test()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();

            // Act
            builder.AddJwt();
            var jwtBearerOptions = builder.Services.BuildServiceProvider().GetRequiredService<IOptionsMonitor<JwtBearerOptions>>().Get(JwtBearerDefaults.AuthenticationScheme);

            // Assert
            Assert.True(jwtBearerOptions.TokenValidationParameters.ValidateIssuer);
            Assert.True(jwtBearerOptions.TokenValidationParameters.ValidateAudience);
            Assert.True(jwtBearerOptions.TokenValidationParameters.ValidateLifetime);
            Assert.Equal(TimeSpan.Zero, jwtBearerOptions.TokenValidationParameters.ClockSkew);
            Assert.True(jwtBearerOptions.TokenValidationParameters.ValidateIssuerSigningKey);
            Assert.Equal(builder.Configuration["JwtSettings:Issuer"], jwtBearerOptions.TokenValidationParameters.ValidIssuer);
            Assert.Equal(builder.Configuration["JwtSettings:Audience"], jwtBearerOptions.TokenValidationParameters.ValidAudience);
            Assert.IsType<SymmetricSecurityKey>(jwtBearerOptions.TokenValidationParameters.IssuerSigningKey);
            Assert.NotNull(jwtBearerOptions.TokenValidationParameters.IssuerSigningKey);
            Assert.Equal(builder.Configuration["JwtSettings:Key"], Encoding.UTF8.GetString(((SymmetricSecurityKey)jwtBearerOptions.TokenValidationParameters.IssuerSigningKey).Key));

        }

    }
}

