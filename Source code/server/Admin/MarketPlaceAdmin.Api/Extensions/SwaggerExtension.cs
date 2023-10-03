using Microsoft.OpenApi.Models;
using System.Reflection;

namespace MarketPlaceAdmin.Api.Extensions
{
    /// <summary>
    /// Extension methods for configuring Swagger/OpenAPI documentation for an ASP.NET Core web application.
    /// </summary>
    public static class SwaggerExtension
    {
        /// <summary>
        /// Adds Swagger/OpenAPI documentation generation services to the application's service collection.
        /// </summary>
        /// <param name="builder">The WebApplicationBuilder instance.</param>
        /// <returns>The WebApplicationBuilder instance with Swagger services added.</returns>
        public static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = $"Marketplace Admin API : {builder.Environment.EnvironmentName}", Version = "Phase 3.1" });
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
                var xfileapi = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xfilebussiness = $"MarketPlaceAdmin.Bussiness.xml";
                option.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xfileapi));
                option.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xfilebussiness));
            });

            return builder;
        }

        /// <summary>
        /// Uses Swagger middleware to serve Swagger UI and JSON documentation.
        /// </summary>
        /// <param name="app">The WebApplication instance.</param>
        /// <returns>The WebApplication instance with Swagger middleware enabled.</returns>
        public static WebApplication UseSwaggerPage(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            return app;
        }
    }
}