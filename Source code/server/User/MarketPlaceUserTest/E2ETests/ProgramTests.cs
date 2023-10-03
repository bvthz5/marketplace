using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MarketPlaceUserTest.E2ETests
{
    public class ProgramTests
    {
        private readonly HttpClient _client;

        public ProgramTests()
        {
            var factory = new WebApplicationFactory<Program>();
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task TestSwaggerUI_Loaded()
        {

            // Act
            var response = await _client.GetAsync("/swagger/index.html");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.NotEmpty(content);

        }

        [Fact]
        public void AddAuthentication_AddsJwtBearerWithOptions()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var builder = new WebHostBuilder()
                .Configure(app => { })
                .ConfigureServices(services =>
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
                            ValidIssuer = configuration["JwtSettings:Issuer"],
                            ValidAudience = configuration["JwtSettings:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]))
                        };
                    });
                });

            // Act
            var server = new TestServer(builder);
            var client = server.CreateClient();

            // Assert
            using var scope = server.Host.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var authenticationSchemeProvider = serviceProvider.GetService<IAuthenticationSchemeProvider>();
            Assert.Contains(authenticationSchemeProvider.GetAllSchemesAsync().Result,
                s => s.Name == JwtBearerDefaults.AuthenticationScheme);
        }

        [Fact]
        public void JwtBearerOptions_Events_OnMessageReceived_SetsTokenCorrectly()
        {
            // Arrange
            var options = new JwtBearerOptions();
            var context = new DefaultHttpContext();
            context.Request.Path = "/notification";
            context.Request.QueryString = new QueryString("?access_token=mytoken");
            var scheme = new AuthenticationScheme("testScheme", "testScheme", typeof(JwtBearerHandler));
            var eventContext = new MessageReceivedContext(context, scheme, options);

            // Act
            options.Events?.OnMessageReceived(eventContext);

            // Assert
            Assert.Null(eventContext.Token);
        }

        [Fact]
        public void JWT_Options_ValidateIssuer_Test()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    builder.Configuration.Bind("JwtSettings", options);
                });
            var jwtBearerOptions = builder.Services.BuildServiceProvider()
                .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
                .Get(JwtBearerDefaults.AuthenticationScheme);

            // Act
            var isValidIssuer = jwtBearerOptions.TokenValidationParameters.ValidateIssuer;

            // Assert
            Assert.True(isValidIssuer);
        }

        [Fact]
        public void JWT_Options_ValidateAudience_Test()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    builder.Configuration.Bind("JwtSettings", options);
                });
            var jwtBearerOptions = builder.Services.BuildServiceProvider()
                .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
                .Get(JwtBearerDefaults.AuthenticationScheme);

            // Act
            var isValidAudience = jwtBearerOptions.TokenValidationParameters.ValidateAudience;

            // Assert
            Assert.True(isValidAudience);
        }
    }
}