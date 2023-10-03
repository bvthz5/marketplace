using MarketPlaceAdmin.Api.Middlewares;
using MarketPlaceAdmin.Bussiness.Interfaces;
using MarketPlaceAdmin.Bussiness.Security.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Net;
using Xunit;


namespace MarketplaceAdminTest.Middlewares
{
    public class AgentValidationMiddlewareTests
    {
        private readonly ILogger<AgentValidationMiddleware> _logger;
        private readonly ISecurityUtil _securityUtil;
        private readonly IAgentService _agentService;

        public AgentValidationMiddlewareTests()
        {
            _logger = Substitute.For<ILogger<AgentValidationMiddleware>>();
            _securityUtil = Substitute.For<ISecurityUtil>();
            _agentService = Substitute.For<IAgentService>();
        }

        [Fact]
        public async Task InvokeAsync_WithValidAgent_ShouldCallNextMiddleware()
        {
            // Arrange
            var middleware = new AgentValidationMiddleware(_logger, _securityUtil, _agentService);
            var context = new DefaultHttpContext();
            _securityUtil.GetCurrentUserId().Returns(1);
            _securityUtil.IsAdmin().Returns(false);
            _agentService.IsValidAgent(1, "/api/agents").Returns(true);

            // Act
            await middleware.InvokeAsync(context, (httpContext) => Task.CompletedTask);

            // Assert
            await _agentService.ReceivedWithAnyArgs(1).IsValidAgent(1, "/api/agents");
        }

        [Fact]
        public async Task InvokeAsync_WithInvalidAgent_ShouldSetUnauthorizedStatusCode()
        {
            // Arrange
            var middleware = new AgentValidationMiddleware(_logger, _securityUtil, _agentService);
            var context = new DefaultHttpContext();
            _securityUtil.GetCurrentUserId().Returns(2);
            _securityUtil.IsAdmin().Returns(false);
            _agentService.IsValidAgent(2, "/api/agents").Returns(false);

            // Act
            await middleware.InvokeAsync(context, (httpContext) => Task.CompletedTask);

            // Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_WithAdminUser_ShouldCallNextMiddleware()
        {
            // Arrange
            var middleware = new AgentValidationMiddleware(_logger, _securityUtil, _agentService);
            var context = new DefaultHttpContext();
            _securityUtil.GetCurrentUserId().Returns(3);
            _securityUtil.IsAdmin().Returns(true);

            // Act
            await middleware.InvokeAsync(context, (httpContext) => Task.CompletedTask);

            // Assert
            await _agentService.DidNotReceive().IsValidAgent(Arg.Any<int>(), Arg.Any<string>());
        }

    }

}
