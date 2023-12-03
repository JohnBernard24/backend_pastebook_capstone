using backend_pastebook_capstone.Data;
using backend_pastebook_capstone.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;

namespace backend_pastebook_capstone.Repository
{
	public class FriendRepository
	{
		private readonly CapstoneDBContext _context;

		public FriendRepository(CapstoneDBContext context)
		{
			_context = context;
		}

		public List<User> GetUserFriendListByUserId(Guid userId)
		{
			var friendIds = _context.Friend
				.Where(f => (f.ReceiverId == userId || f.SenderId == userId) && f.IsFriend)
				.Select(f => f.SenderId == userId ? f.ReceiverId : f.SenderId)
				.Distinct()
				.ToList();

			List<User>? friends = _context.User
				.Where(u => friendIds.Contains(u.Id))
				.ToList();

			return friends;
		}

		public List<Friend> GetFriendListByUserId(Guid userId)
		{
			return _context.Friend
				.Where(f => f.ReceiverId == userId && f.IsFriend == true)
				.ToList();
		}

		public int GetFriendCountByUserId(Guid userId)
		{
			var friendCount = _context.Friend
				.Where(f => (f.ReceiverId == userId || f.SenderId == userId) && f.IsFriend == true)
				.Select(f => f.SenderId)
				.Distinct()
				.Count();

			return friendCount;
		}

		public List<Friend> GetFriendRequestList(Guid userId)
		{
			List<Friend>? friendRequests = _context.Friend
				.Where(f => f.ReceiverId == userId && f.IsFriend == false)
				.Include(f => f.Receiver)
				.Include(f => f.Sender)
				.ToList();

			return friendRequests;
		}

		public bool FriendRequestExists(Guid senderId, Guid receiverId)
		{
			return _context.Friend
				.Any(f =>
					(f.SenderId == senderId && f.ReceiverId == receiverId) ||
					(f.SenderId == receiverId && f.ReceiverId == senderId)
				);
		}

		public Friend? FriendExist(Guid senderId, Guid receiverId)
		{
			return _context.Friend
				.FirstOrDefault(f =>
					(f.SenderId == senderId && f.ReceiverId == receiverId) ||
					(f.SenderId == receiverId && f.ReceiverId == senderId)
				);
		}

		public Friend? GetFriendByCurrentUserAndUserToRemoveId(Guid currentUserId, Guid userToRemoveId)
		{
			return _context.Friend
				.FirstOrDefault(f =>
				(f.SenderId == currentUserId && f.ReceiverId == userToRemoveId) ||
				(f.SenderId == userToRemoveId && f.ReceiverId == currentUserId)
			);
		}

		public Friend? GetFriendRequestByFriendId(Guid requestId)
		{
			return _context.Friend.Find(requestId);
		}


		public void AddFriend(Friend friend)
		{
			_context.Friend.Add(friend);
			_context.SaveChanges();
		}
		public void UpdateFriend(Friend friend)
		{
			_context.Friend.Update(friend);
			_context.SaveChanges();
		}
		public void RemoveFriend(Friend friend)
		{
			_context.Friend.Remove(friend);
			_context.SaveChanges();
		}

	}
}
