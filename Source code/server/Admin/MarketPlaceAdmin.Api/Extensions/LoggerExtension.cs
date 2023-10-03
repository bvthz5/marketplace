using Serilog;

namespace MarketPlaceAdmin.Api.Extensions
{
    /// <summary>
    /// Extension methods for configuring Serilog logger in a web application builder.
    /// </summary>
    public static class LoggerExtension
    {
        /// <summary>
        /// Adds Serilog logger to the web application builder.
        /// </summary>
        /// <param name="builder">The web application builder.</param>
        /// <returns>The updated web application builder.</returns>
        public static WebApplicationBuilder AddCustomLogger(this WebApplicationBuilder builder)
        {
            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            // Remove any existing logging providers and add Serilog.
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);

            return builder;
        }
    }
}
