using backend_pastebook_capstone.Data;
using backend_pastebook_capstone.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_pastebook_capstone.Repository
{
	public class NotificationRepository
	{
		private readonly CapstoneDBContext _context;

		public NotificationRepository(CapstoneDBContext context)
		{
			_context = context;
		}

		public List<Notification> GetAllNotificationByUserId(Guid userId)
		{
			return _context.Notification.Where(n => n.NotifiedUserId == userId)
				.Include(n => n.NotifiedUser)
				.ToList();
		}

		public Notification? GetNotificationByNotificationId(Guid notificationId)
		{
			return _context.Notification.Include(n => n.NotifiedUser).FirstOrDefault(n => n.Id == notificationId);
		}

		public Notification? GetNotificationByContextIdAndNotificationType(Guid contextId, string notificationType)
		{
			return _context.Notification.FirstOrDefault(n => n.ContextId == contextId && n.NotificationType == notificationType);
		}

		public Like? GetLikeByContextId(Guid contextId)
		{
			return _context.Like.Include(l => l.Post).Include(l => l.Liker).FirstOrDefault(l => l.Id == contextId);
		}

		public Comment? GetCommentByContextId(Guid contextId)
		{
			return _context.Comment.Include(c => c.Post).Include(c => c.Commenter).FirstOrDefault(c => c.Id == contextId);
		}

		public Friend? GetFriendRequestByContextId(Guid contextId)
		{
			return _context.Friend.Include(f => f.Sender).FirstOrDefault(f => f.Id == contextId);
		}

		public void AddNotification(Notification notification)
		{
			_context.Notification.Add(notification);
			_context.SaveChanges();
		}

		public void RemoveNotification(Notification notification)
		{
			_context.Notification.Remove(notification);
			_context.SaveChanges();
		}
	}
}
