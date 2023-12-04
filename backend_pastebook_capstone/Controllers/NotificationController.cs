using backend_pastebook_capstone.Models;
using backend_pastebook_capstone.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace backend_pastebook_capstone.Controllers
{
	[Route("api/notification")]
	[ApiController]
	public class NotificationController : ControllerBase
	{
		private readonly NotificationRepository _notificationRepository;
		private readonly UserRepository _userRepository;

		public NotificationController(NotificationRepository notificationRepository, UserRepository userRepository)
		{
			_notificationRepository = notificationRepository;
			_userRepository = userRepository;
		}

		[HttpGet("get-notifications")]
		public ActionResult<IEnumerable<NotificationDTO>> GetAllNotifications()
		{
			string? token = Request.Headers["Authorization"];
			if (token == null || _userRepository.GetUserByToken(token) == null)
				return BadRequest(new { result = "no_token_sent" });

			User? user = _userRepository.GetUserByToken(token);
			if (user == null)
				return BadRequest(new { result = "no_user_found" });

			List<Notification>? notifications = _notificationRepository.GetNotificationListByUserId(user.Id);
			if (notifications == null)
				return NotFound(new { result = "no_notifications_found" });

			List<NotificationDTO> notificationDTOs = new List<NotificationDTO>();
			foreach (Notification notification in notifications)
			{
				if (notification.NotifiedUser == null)
					continue;

				notificationDTOs.Add(new NotificationDTO
				{
					Id = notification.Id,
					NotifiedUserId = notification.NotifiedUserId,
					NotifiedUser = new MiniProfileDTO
					{
						Id = notification.NotifiedUser.Id,
						FirstName = notification.NotifiedUser.FirstName,
						LastName = notification.NotifiedUser.LastName,
						Photo = notification.NotifiedUser.Photo
					},
					NotificationType = notification.NotificationType,
					ContextId = notification.ContextId,
					IsRead = notification.IsRead
				});
			}

			return notificationDTOs;
		}

		[HttpGet("get-notification-context/{notificationId}")]
		public IActionResult GetNotificationContext(Guid notificationId)
		{
			string? token = Request.Headers["Authorization"];
			if (token == null || _userRepository.GetUserByToken(token) == null)
				return BadRequest(new { result = "no_token_sent" });

			Notification? notification = _notificationRepository.GetNotificationByNotificationId(notificationId);
			if (notification == null)
				return NotFound(new { result = "no_notification_found" });

			switch (notification.NotificationType)
			{
				case "like":
					Like? like = _notificationRepository.GetLikeByContextId(notification.ContextId);
					return SerializeAndReturn(like);

				case "comment":
					Comment? comment = _notificationRepository.GetCommentByContextId(notification.ContextId);
					return SerializeAndReturn(comment);

				case "add-friend-request":
				case "accept-friend-request":
					Friend? friend = _notificationRepository.GetSentFriendRequestByContextId(notification.ContextId);
					return SerializeAndReturn(friend);

				default:
					return BadRequest(new { result = "notification_type_invalid" });
			}
		}

		[HttpPut("update-notification-read/{notificationId}")]
		public IActionResult UpdateNotificationRead(Guid notificationId)
		{
			string? token = Request.Headers["Authorization"];
			if (token == null || _userRepository.GetUserByToken(token) == null)
				return BadRequest(new { result = "no_token_sent" });

			Notification? notification = _notificationRepository.GetNotificationByNotificationId(notificationId);
			if (notification == null)
				return NotFound(new { result = "no_notification_found" });

			notification.IsRead = true;

			_notificationRepository.UpdateNotification(notification);

			return Ok(notification);
		}

		private ActionResult SerializeAndReturn(object data)
		{
			string jsonData = JsonConvert.SerializeObject(data);
			return new ContentResult
			{
				Content = jsonData,
				ContentType = "application/json",
				StatusCode = 200
			};
		}


		[HttpDelete("delete-notification/{notificationId}")]
		public IActionResult DeleteNotification(Guid notificationId)
		{
			string? token = Request.Headers["Authorization"];
            if (token == null || _userRepository.GetUserByToken(token) == null)
                return BadRequest(new { result = "no_token_sent" });

            Notification? notification = _notificationRepository.GetNotificationByNotificationId(notificationId);
            if (notification == null)
                return NotFound(new { result = "no_notification_found" });

			_notificationRepository.RemoveNotification(notification);

			return Ok(new { result = "notification successfully deleted" });
        }
	}
}
