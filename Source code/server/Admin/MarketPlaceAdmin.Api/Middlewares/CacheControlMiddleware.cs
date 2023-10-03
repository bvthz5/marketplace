namespace MarketPlaceAdmin.Api.Middlewares
{
    /// <summary>
    /// Middleware component that sets the Cache-Control header to prevent caching.
    /// </summary>
    public class CacheControlMiddleware : IMiddleware
    {
        /// <summary>
        /// Sets the Cache-Control header to "no-cache, no-store, must-revalidate" and passes control to the next middleware component in the pipeline.
        /// </summary>
        /// <param name="context">The HttpContext for the current request.</param>
        /// <param name="next">The next middleware component in the pipeline.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            context.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");

            await next(context);
        }
    }
}