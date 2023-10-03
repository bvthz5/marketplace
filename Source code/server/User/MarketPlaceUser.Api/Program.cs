using MarketPlace.DataAccess.Data;
using MarketPlace.DataAccess.Interfaces;
using MarketPlaceUser.Api.Middlewares;
using MarketPlaceUser.Bussiness.Hubs;
using MarketPlaceUser.Bussiness.Interfaces;
using MarketPlaceUser.Bussiness.Security;
using MarketPlaceUser.Bussiness.Services;
using MarketPlaceUser.Bussiness.Settings;
using MarketPlaceUser.Bussiness.Util;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();

//Sql Connection
builder.Services.AddDbContext<MarketPlaceDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<IPhotosService, PhotosService>();

builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<IRazorpayService, RazorpayService>();

builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<IWishListService, WishListService>();

builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddScoped<IDeliveryAddressService, DeliveryAddressService>();

builder.Services.AddScoped<IOrderDetailsService, OrderDetailsService>();

builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddScoped<IRefundService, RefundService>();

builder.Services.AddScoped<GoogleAuth>();

builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();

builder.Services.AddScoped<ISecurityUtil, SecurityUtil>();

builder.Services.AddScoped<IFileUtil, FileUtil>();

builder.Services.AddSingleton<PresenceTracker>();

//Middleware Registration
builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();

builder.Services.AddTransient<CacheControlMiddleware>();

builder.Services.AddTransient<ValidateUserMiddleware>();

// Google Credentials
builder.Services.Configure<GoogleAuthSettings>(builder.Configuration.GetSection("GoogleAuthSettings"));

// Mail Settings
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

// Jwt Settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Image Settings
builder.Services.Configure<ImageSettings>(builder.Configuration.GetSection("ImageSettings"));

builder.Services.Configure<RazorPaySettings>(builder.Configuration.GetSection("RazorPaySettings"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(option =>
{

    option.SwaggerDoc("v1", new OpenApiInfo { Title = $"Marketplace User API : {builder.Environment.EnvironmentName}", Version = "Phase 3.1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            Array.Empty<string>()
        }

    });

});

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

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            // If the request is for our hub...
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/notification"))
            {
                // Read the token out of the query string
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});
var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>())
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSignalR();

builder.Services.AddHealthChecks();

var app = builder.Build();

//Run Migration Script On Startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MarketPlaceDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseHsts();

app.UseCors();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseMiddleware<CacheControlMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<ValidateUserMiddleware>();

app.MapHealthChecks("/health");

app.MapHub<NotificationHub>("/notification");

app.MapControllers();

app.Run();