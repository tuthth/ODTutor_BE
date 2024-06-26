using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;
using Models.Models.Requests;
using Models.Models.Views;
using Services.Interfaces;

namespace API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // POST: api/notification
        [HttpPost("create")]
        public async Task<IActionResult> CreateNotification([FromBody] NotificationRequest notificationRequest)
        {
            if (notificationRequest == null)
            {
                return BadRequest(); // 400 Bad Request
            }

            var response = await _notificationService.CreateNotification(notificationRequest);
            return response;
        }

        /// <summary>
        /// Get All Notification By UserId
        /// </summary>
        [HttpGet("get/{userId}")]
        public async Task<List<NotificationResponse>> GetNotificationByUserId( Guid userId)
        {
            var response = await _notificationService.GetNotificationByUserId(userId);
            return response;
        }

        /// <summary>
        /// Update Read or Unread Notification
        /// </summary>
        [HttpPut("update/{userId}/{notificationId}")]
        public async Task<IActionResult> SetNotificationStatus(Guid userId, Guid notificationId)
        {
            var response = await _notificationService.SetNotificationStatus(userId, notificationId);
            return response;
        }
    }

}
