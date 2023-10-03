using MarketPlaceUser.Bussiness.Interfaces;
using MarketPlaceUser.Bussiness.Security;
using System.Net;

namespace MarketPlaceUser.Api.Middlewares
{
    public class ValidateUserMiddleware : IMiddleware
    {
        private readonly ILogger<ValidateUserMiddleware> _logger;

        private readonly ISecurityUtil _securityUtil;

        private readonly IUserService _userService;

        public ValidateUserMiddleware(ILogger<ValidateUserMiddleware> logger, ISecurityUtil securityUtil, IUserService userService)
        {
            _logger = logger;
            _securityUtil = securityUtil;
            _userService = userService;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var userId = _securityUtil.GetCurrentUserId();

            // If userId is not valid and have an active status 
            // Short circuit the request pipeline and return Unauthorized StatusCode
            if (userId != 0 && !await _userService.IsValidActiveUser(userId))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                _logger.LogWarning("User Access Denied User Id : {userId}", userId);
            }
            else
            {
                await next(context);
            }

        }
    }
}
