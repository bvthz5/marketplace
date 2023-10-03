using MarketPlaceAdmin.Api.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Net;
using System.Text.Json;
using Xunit;

namespace MarketplaceAdminTest.Middlewares
{
    public class GlobalExceptionHandlingMiddlewareTests
    {
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
        private readonly HttpContext _httpContext;

        public GlobalExceptionHandlingMiddlewareTests()
        {
            _logger = Substitute.For<ILogger<GlobalExceptionHandlingMiddleware>>();
            _httpContext = new DefaultHttpContext();
        }

        [Fact]
        public async Task InvokeAsync_Should_Not_Throw_Exception()
        {
            // Arrange
            var middleware = new GlobalExceptionHandlingMiddleware(_logger);

            var next = new RequestDelegate(_ => Task.CompletedTask);

            // Act
            await middleware.InvokeAsync(_httpContext, next);

            // Assert
            Assert.NotEqual((int)HttpStatusCode.InternalServerError, _httpContext.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_WhenUnhandledExceptionIsThrown_ShouldReturnInternalServerError()
        {
            // Arrange
            var middleware = new GlobalExceptionHandlingMiddleware(_logger);

            Exception ex = new("Something Went Wrong");

            _httpContext.Response.Body = new MemoryStream();

            var next = new RequestDelegate(_ => throw ex);

            // Act
            await middleware.InvokeAsync(_httpContext, next);

            // Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, _httpContext.Response.StatusCode);
            Assert.Equal("application/json", _httpContext.Response.ContentType);

            _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);

            _logger.ReceivedWithAnyArgs().LogError("{e} {message}", ex, ex.Message);
        }

        [Fact]
        public async Task InvokeAsync_Unauthorized_ReturnsInvalidTokenResponse()
        {
            // Arrange
            var middleware = new GlobalExceptionHandlingMiddleware(_logger);

            var next = Substitute.For<RequestDelegate>();
            _httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            _httpContext.Response.Headers.WWWAuthenticate = "Bearer error=\"invalid_token\"";
            _httpContext.Response.Body = new MemoryStream();

            // Act
            await middleware.InvokeAsync(_httpContext, next);

            // Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, _httpContext.Response.StatusCode);
            Assert.Equal("application/json", _httpContext.Response.ContentType);

            _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();

            var error = JsonSerializer.Deserialize<JsonElement>(responseBody);

            Assert.IsType<JsonElement>(error);
            Assert.Equal((int)HttpStatusCode.Unauthorized, int.Parse(error.GetProperty("serviceStatus").ToString()));
            Assert.Equal("Invalid Token", error.GetProperty("message").ToString());
            Assert.False(bool.Parse(error.GetProperty("status").ToString()));
        }

        [Fact]
        public async Task InvokeAsync_Unauthorized_ReturnsTokenExpiredResponse()
        {
            // Arrange
            var middleware = new GlobalExceptionHandlingMiddleware(_logger);

            var next = Substitute.For<RequestDelegate>();
            _httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            _httpContext.Response.Headers.WWWAuthenticate = "Bearer error= token expired ";
            _httpContext.Response.Body = new MemoryStream();

            // Act
            await middleware.InvokeAsync(_httpContext, next);

            // Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, _httpContext.Response.StatusCode);
            Assert.Equal("application/json", _httpContext.Response.ContentType);

            _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();

            var error = JsonSerializer.Deserialize<JsonElement>(responseBody);

            Assert.Equal((int)HttpStatusCode.Unauthorized, int.Parse(error.GetProperty("serviceStatus").ToString()));
            Assert.Equal("Token Expired", error.GetProperty("message").ToString());
            Assert.False(bool.Parse(error.GetProperty("status").ToString()));
        }

        [Fact]
        public async Task InvokeAsync_Unauthorized_ReturnsUnauthorizedResponse()
        {
            // Arrange
            var middleware = new GlobalExceptionHandlingMiddleware(_logger);

            var next = Substitute.For<RequestDelegate>();
            _httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            _httpContext.Response.Headers.WWWAuthenticate = "Bearer error";
            _httpContext.Response.Body = new MemoryStream();

            // Act
            await middleware.InvokeAsync(_httpContext, next);

            // Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, _httpContext.Response.StatusCode);
            Assert.Equal("application/json", _httpContext.Response.ContentType);

            _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();

            var error = JsonSerializer.Deserialize<JsonElement>(responseBody);

            Assert.Equal((int)HttpStatusCode.Unauthorized, int.Parse(error.GetProperty("serviceStatus").ToString()));
            Assert.Equal("Unauthorized", error.GetProperty("message").ToString());
            Assert.False(bool.Parse(error.GetProperty("status").ToString()));
        }

        [Fact]
        public async Task InvokeAsync_Forbidden_ReturnsForbiddenResponse()
        {
            // Arrange
            var middleware = new GlobalExceptionHandlingMiddleware(_logger);

            var next = Substitute.For<RequestDelegate>();
            _httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            _httpContext.Response.Headers.WWWAuthenticate = "Bearer error";
            _httpContext.Response.Body = new MemoryStream();

            // Act
            await middleware.InvokeAsync(_httpContext, next);

            // Assert
            Assert.Equal((int)HttpStatusCode.Forbidden, _httpContext.Response.StatusCode);
            Assert.Equal("application/json", _httpContext.Response.ContentType);

            _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();

            var error = JsonSerializer.Deserialize<JsonElement>(responseBody);

            Assert.Equal((int)HttpStatusCode.Forbidden, int.Parse(error.GetProperty("serviceStatus").ToString()));
            Assert.Equal("Forbidden", error.GetProperty("message").ToString());
            Assert.False(bool.Parse(error.GetProperty("status").ToString()));
        }
    }
}