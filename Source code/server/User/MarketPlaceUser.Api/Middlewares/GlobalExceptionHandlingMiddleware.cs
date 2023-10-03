using Newtonsoft.Json;
using System.Net;

namespace MarketPlaceUser.Api.Middlewares
{
    /// <summary>
    /// Middleware for handling global exceptions in the application.
    /// </summary>
    public class GlobalExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalExceptionHandlingMiddleware"/> class.
        /// </summary>
        /// <param name="logger">The logger to use for logging exceptions.</param>
        public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware for handling global exceptions in the application.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        /// <param name="next">The delegate representing the next middleware in the pipeline.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);

                if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    string? token = context.Response.Headers.WWWAuthenticate;

                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        string message;

                        if (token.Contains(" token expired "))
                            message = "Token Expired";

                        else if (token.Contains("invalid_token"))
                            message = "Invalid Token";
                        else
                            message = "Unauthorized";

                        var errorMessage = new
                        {
                            data = (object?)null,
                            message,
                            serviceStatus = 401,
                            status = false
                        };

                        context.Response.ContentType = "application/json";

                        await context.Response.WriteAsync(JsonConvert.SerializeObject(errorMessage));
                    }
                }

                else if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
                {
                    var errorMessage = new
                    {
                        data = (object?)null,
                        message = "Forbidden",
                        serviceStatus = 403,
                        status = false
                    };

                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync(JsonConvert.SerializeObject(errorMessage));
                }

            }
            catch (Exception e)
            {
                _logger.LogError("{e} {message}", e, e.Message);

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var errorMessage = new
                {
                    data = (object?)null,
                    message = "Internal Server Error",
                    serviceStatus = 500,
                    status = false
                };

                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonConvert.SerializeObject(errorMessage));
            }
        }
    }
}
