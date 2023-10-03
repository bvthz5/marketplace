using MarketPlace.DataAccess.Data;
using MarketPlace.DataAccess.Interfaces;
using MarketPlaceAdmin.Bussiness.Interfaces;
using MarketPlaceAdmin.Bussiness.Security;
using MarketPlaceAdmin.Bussiness.Security.Interfaces;
using MarketPlaceAdmin.Bussiness.Services;
using MarketPlaceAdmin.Bussiness.Settings;
using MarketPlaceAdmin.Bussiness.Util;
using MarketPlaceAdmin.Bussiness.Util.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MarketPlaceAdmin.Api.Extensions
{
    /// <summary>
    /// Provides extension methods to add various services to the WebApplicationBuilder.
    /// </summary>
    public static class ServicesExtentions
    {
        /// <summary>
        /// Adds a SQL Server database context to the WebApplicationBuilder using the provided connection string.
        /// </summary>
        /// <param name="builder">The WebApplicationBuilder to add the SQL Server database context to.</param>
        /// <param name="connectionString">The connection string to use for the SQL Server database context.</param>
        /// <returns>The modified WebApplicationBuilder.</returns>
        public static WebApplicationBuilder AddSqlConnection(this WebApplicationBuilder builder, string? connectionString)
        {
            //Sql Connection
            builder.Services.AddDbContext<MarketPlaceDbContext>(options => options.UseSqlServer(connectionString));

            return builder;
        }

        /// <summary>
        /// Registers all services used by the application. This method adds the necessary dependencies to the application's service collection.
        /// </summary>
        /// <param name="builder">The <see cref="WebApplicationBuilder"/> to add the services to.</param>
        /// <returns>The updated <see cref="WebApplicationBuilder"/> instance.</returns>
        public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
        {
            // Add scoped services
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IPhotosService, PhotosService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IAdminOrderService, AdminOrderService>();
            builder.Services.AddScoped<IAgentService, AgentService>();
            builder.Services.AddScoped<IAgentOrderService, AgentOrderService>();

            // Add util services
            builder.Services.AddScoped<ISecurityUtil, SecurityUtil>();
            builder.Services.AddScoped<IGoogleAuth, GoogleAuth>();
            builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();
            builder.Services.AddScoped<IFileUtil, FileUtil>();

            return builder;
        }

        /// <summary>
        /// Configures various application settings.
        /// </summary>
        /// <param name="builder">The WebApplicationBuilder to configure settings for.</param>
        /// <returns>The modified WebApplicationBuilder.</returns>
        public static WebApplicationBuilder ConfigureSettings(this WebApplicationBuilder builder)
        {
            // Google Credentials
            builder.Services.Configure<GoogleAuthSettings>(builder.Configuration.GetSection("GoogleAuthSettings"));

            // Mail Settings
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

            // Jwt Settings
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

            // Image Settings
            builder.Services.Configure<ImageSettings>(builder.Configuration.GetSection("ImageSettings"));

            return builder;
        }

        /// <summary>
        /// Adds JWT authentication to the WebApplicationBuilder.
        /// </summary>
        /// <param name="builder">The WebApplicationBuilder to add JWT authentication to.</param>
        /// <returns>The modified WebApplicationBuilder.</returns>
        public static WebApplicationBuilder AddJwt(this WebApplicationBuilder builder)
        {
            //JWT Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
                };
            });

            return builder;
        }
    }
}
