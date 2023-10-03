using MarketPlaceAdmin.Bussiness.Interfaces;
using MarketPlaceAdmin.Bussiness.Security.Interfaces;
using System.Net;

namespace MarketPlaceAdmin.Api.Middlewares
{
    /// <summary>
    /// Middleware that validates the user accessing the API endpoints is an authenticated agent and has the necessary permissions to access the resource.
    /// </summary>
    public class AgentValidationMiddleware : IMiddleware
    {
        private readonly ILogger<AgentValidationMiddleware> _logger;
        private readonly ISecurityUtil _securityUtil;
        private readonly IAgentService _agentService;

        /// <summary>
        /// Constructor for the AgentValidationMiddleware class.
        /// </summary>
        /// <param name="logger">Logger for logging messages.</param>
        /// <param name="securityUtil">Utility for retrieving security-related information.</param>
        /// <param name="agentService">Service for retrieving agent-related information.</param>

        public AgentValidationMiddleware(ILogger<AgentValidationMiddleware> logger, ISecurityUtil securityUtil, IAgentService agentService)
        {
            _logger = logger;
            _securityUtil = securityUtil;
            _agentService = agentService;
        }

        /// <summary>
        /// Validates if the user accessing the API endpoints is an authenticated agent and has the necessary permissions to access the resource.
        /// </summary>
        /// <param name="context">HTTP context of the incoming request.</param>
        /// <param name="next">The next middleware in the pipeline.</param>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // Retrieve the user ID of the current user from the security utility.
            int userId = _securityUtil.GetCurrentUserId();

            string path = context.Request.Path;

            // Check if the current request is for changing the password, the user is not an admin or the user ID is 0.
            if (!_securityUtil.IsAdmin() && userId != 0 && !await _agentService.IsValidAgent(userId, path))
            {
                // Set the HTTP status code to Unauthorized.
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                // Log a warning message indicating that the user was denied access.
                _logger.LogWarning("Agent Access Denied Agent Id : {userId}", userId);
            }
            else
            {
                // Pass the HTTP context to the next middleware in the pipeline.
                await next(context);
            }
        }
    }
}
