using MarketPlace.DataAccess.Data;
using MarketPlaceAdmin.Api.Extensions;
using Microsoft.EntityFrameworkCore;

// Create a new instance of WebApplicationBuilder to build the web application.
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Register the controllers with the dependency injection container.
builder.Services.AddControllers();

// Add an HTTP context accessor to the dependency injection container.
builder.Services.AddHttpContextAccessor();

// Add a database connection to the dependency injection container.
builder.AddSqlConnection(builder.Configuration.GetConnectionString("Default"));

// Register custom services with the dependency injection container.
builder.RegisterServices();

// Configure custom settings for the web application.
builder.ConfigureSettings();

// Add custom middleware components to the web application pipeline.
builder.AddMiddlewares();

// Add a custom logger to the web application.
builder.AddCustomLogger();

// Add Swagger documentation to the web application.
builder.AddSwagger();

// Add JWT authentication to the web application.
builder.AddJwt();

// Add health checks
builder.Services.AddHealthChecks();

// Add CORS policy to the web application.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>())
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Build the web application.
var app = builder.Build();

// Run database migration on startup.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MarketPlaceDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.

// Add Swagger UI to the web application.
app.UseSwaggerPage();

// Redirect HTTP requests to HTTPS.
app.UseHttpsRedirection();

// Enable HTTP strict transport security (HSTS).
app.UseHsts();

// Enable CORS.
app.UseCors();

// Add custom error handling middleware components to the web application pipeline.
app.UseGlobalExceptionHandlingMiddleware();

// Add cache control middleware components to the web application pipeline.
app.UseCacheControlMiddleware();

// Enable authentication
app.UseAuthentication();

// Enable authorization.
app.UseAuthorization();

// Add Agent Validation
app.UseAgentValidationMiddleware();

// Configure health check endpoint
app.MapHealthChecks("/health");

// Map the controllers to routes.
app.MapControllers();

// Start the web application.
app.Run();