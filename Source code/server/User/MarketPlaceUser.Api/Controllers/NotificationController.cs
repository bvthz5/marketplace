using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;
using MarketPlaceUser.Bussiness.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlaceUser.Api.Controllers
{
    /// <summary>
    /// API controller for managing notifications.
    /// </summary>
    [Route("api/notification")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ISecurityUtil _securityUtil;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationController"/> class.
        /// </summary>
        /// <param name="notificationService">The notification service.</param>
        /// <param name="securityUtil">The security utility.</param>
        public NotificationController(INotificationService notificationService, ISecurityUtil securityUtil)
        {
            _notificationService = notificationService;
            _securityUtil = securityUtil;
        }

        /// <summary>
        /// Gets the notifications for the current user.
        /// </summary>
        /// <returns>The notifications for the current user.</returns>
        [Authorize]
        [HttpGet(Name = "GetNotifications")]
        public async Task<IActionResult> GetNotifications()
        {
            ServiceResult result = await _notificationService.GetNotificationsByUserId(_securityUtil.GetCurrentUserId());
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Gets the number of unread notifications for the current user.
        /// </summary>
        /// <returns>The number of unread notifications for the current user.</returns>
        [Authorize]
        [HttpGet("count", Name = "GetUnreadNotificationCount")]
        public async Task<IActionResult> GetUnreadNotificationCount()
        {
            ServiceResult result = await _notificationService.GetUnreadNotificationCount(_securityUtil.GetCurrentUserId());
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Marks all notifications as read for the current user.
        /// </summary>
        /// <returns>A message indicating whether the operation was successful.</returns>
        [Authorize]
        [HttpPut(Name = "MarkAsRead")]
        public async Task<IActionResult> MarkAsRead()
        {
            ServiceResult result = await _notificationService.MarkAsRead(_securityUtil.GetCurrentUserId());
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Deletes all notifications for the current user.
        /// </summary>
        /// <returns>A message indicating whether the operation was successful.</returns>
        [Authorize]
        [HttpDelete(Name = "DeleteAllNotifications")]
        public async Task<IActionResult> DeleteAllNotifications()
        {
            ServiceResult result = await _notificationService.DeleteAllNotificationsByUserId(_securityUtil.GetCurrentUserId());
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Deletes a notification for the current user by ID.
        /// </summary>
        /// <param name="notificationId">The ID of the notification to delete.</param>
        /// <returns>A message indicating whether the operation was successful.</returns>
        [Authorize]
        [HttpDelete("{notificationId}", Name = "DeleteNotification")]
        public async Task<IActionResult> DeleteNotification(int notificationId)
        {
            ServiceResult result = await _notificationService.DeleteNotification(_securityUtil.GetCurrentUserId(), notificationId);
            return StatusCode((int)result.ServiceStatus, result);
        }
    }
}