using backend_pastebook_capstone.AuthenticationService.Models;
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

		public List<Notification> GetNotificationListByUserId(Guid userId)
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
			return _context.Like.Include(l => l.Post).Include(l => l.Liker).ThenInclude(l => l!.Photo).FirstOrDefault(l => l.Id == contextId);
		}

		public Comment? GetCommentByContextId(Guid contextId)
		{
			return _context.Comment.Include(c => c.Post).Include(c => c.Commenter).ThenInclude(c => c!.Photo).FirstOrDefault(c => c.Id == contextId);
		}

		public Friend? GetSentFriendRequestByContextId(Guid contextId)
		{
			return _context.Friend.Include(f => f.Sender).ThenInclude(f => f!.Photo).Include(f => f.Receiver).ThenInclude(f => f!.Photo).FirstOrDefault(f => f.Id == contextId);
		}

		// following function is to be used when accessing the notification that says "___ has accepted your friend request
		public Friend? GetAcceptedFriendRequestByContextId(Guid contextId)
		{
			return _context.Friend.Include(f => f.Receiver).Include(f => f.Sender).FirstOrDefault(f => f.Id == contextId);
		}

		public void ClearNotificationByUserId(Guid userId)
		{
            IEnumerable<Notification> notifications = _context.Notification.ToArray().Where(t => t.NotifiedUserId == userId).ToList();
            
			_context.Notification.RemoveRange(notifications);
            _context.SaveChanges();
        }
        

        public void AddNotification(Notification notification)
		{
			_context.Notification.Add(notification);
			_context.SaveChanges();
		}

		public void UpdateNotification(Notification notification)
		{
			_context.Notification.Update(notification);
			_context.SaveChanges();
		}

		public void RemoveNotification(Notification notification)
		{
			_context.Notification.Remove(notification);
			_context.SaveChanges();
		}
	}
}
