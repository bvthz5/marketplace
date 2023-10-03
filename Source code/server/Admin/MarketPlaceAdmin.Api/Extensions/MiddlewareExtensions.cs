using MarketPlaceAdmin.Api.Middlewares;

namespace MarketPlaceAdmin.Api.Extensions
{
    /// <summary>
    /// Provides extension methods for registering and using middlewares in a <see cref="WebApplicationBuilder"/>.
    /// </summary>
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Registers middlewares in the specified <see cref="WebApplicationBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="WebApplicationBuilder"/> to register middlewares in.</param>
        /// <returns>The <see cref="WebApplicationBuilder"/> with middlewares registered.</returns>
        public static WebApplicationBuilder AddMiddlewares(this WebApplicationBuilder builder)
        {
            //Middleware Registration
            builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();
            builder.Services.AddTransient<CacheControlMiddleware>();
            builder.Services.AddTransient<AgentValidationMiddleware>();

            return builder;
        }

        /// <summary>
        /// Adds exception handling middlewares to the specified <see cref="WebApplication"/>.
        /// </summary>
        /// <param name="app">The <see cref="WebApplication"/> to add middlewares to.</param>
        /// <returns>The <see cref="WebApplication"/> with middlewares added.</returns>
        public static WebApplication UseGlobalExceptionHandlingMiddleware(this WebApplication app)
        {
            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
            return app;
        }

        /// <summary>
        /// Extension method to add the CacheControlMiddleware to the middleware pipeline.
        /// </summary>
        /// <param name="app">The WebApplication to configure.</param>
        /// <returns>The configured WebApplication.</returns>
        public static WebApplication UseCacheControlMiddleware(this WebApplication app)
        {
            app.UseMiddleware<CacheControlMiddleware>();
            return app;
        }

        /// <summary>
        /// Extension method for adding the AgentValidationMiddleware to the application middleware pipeline.
        /// </summary>
        /// <param name="app">The web application instance.</param>
        /// <returns>The web application instance with the middleware added to the pipeline.</returns>
        public static WebApplication UseAgentValidationMiddleware(this WebApplication app)
        {
            app.UseMiddleware<AgentValidationMiddleware>();
            return app;
        }
    }
}