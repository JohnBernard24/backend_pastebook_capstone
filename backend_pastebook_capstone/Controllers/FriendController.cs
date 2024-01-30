using backend_pastebook_capstone.Models;
using backend_pastebook_capstone.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend_pastebook_capstone.Controllers
{
	[Route("api/friend")]
	[ApiController]
	public class FriendController : ControllerBase
	{
		private readonly UserRepository _userRepository;
		private readonly FriendRepository _friendRepository;
		private readonly NotificationRepository _notificationRepository;

		public FriendController(UserRepository userRepository, FriendRepository friendRepository, NotificationRepository notificationRepository)
		{
			_userRepository = userRepository;
			_friendRepository = friendRepository;
			_notificationRepository = notificationRepository;
		}


		[HttpPost("add-friend")]
		public IActionResult AddFriendRequest([FromBody] FriendDTO friendDTO)
		{
			string? token = Request.Headers["Authorization"];
			if (token == null || _userRepository.GetUserByToken(token) == null)
				return BadRequest(new { result = "no_token_sent" });

			User? receiver = _userRepository.GetUserById(friendDTO.ReceiverId);
			if (receiver == null)
				return BadRequest(new { result = "receiver_not_valid" });

			//here we used the token because the sender of the add friend is always the one who is logged in
			User? sender = _userRepository.GetUserByToken(token);
			if (sender == null)
				return BadRequest(new { result = "sender_not_valid" });

			bool exisitingRequest = _friendRepository.FriendRequestExists(friendDTO.SenderId, friendDTO.ReceiverId);

			if (exisitingRequest)
				return BadRequest(new { result = "request_already_exists" });

			Friend friendRequest = new Friend
			{
				SenderId = friendDTO.SenderId,
				Sender = sender,
				ReceiverId = friendDTO.ReceiverId,
				Receiver = receiver,
				FriendshipDate = null
			};

			_friendRepository.AddFriend(friendRequest);

			Notification friendNotif = new Notification
			{
				NotificationType = "add-friend-request",
				NotifiedUserId = friendRequest.ReceiverId,
				NotifiedUser = friendRequest.Receiver,
				ContextId = friendRequest.Id,
				IsRead = false
			};

			_notificationRepository.AddNotification(friendNotif);

			return Ok(new { result = "friend_request_successfully" });
		}


		[HttpPost("accept-friend/{requestId}")]
		public IActionResult AcceptFriendRequest(Guid requestId)
		{
			string? token = Request.Headers["Authorization"];
			if (token == null || _userRepository.GetUserByToken(token) == null)
				return BadRequest(new { result = "no_token_sent" });

			Friend? friendRequest = _friendRepository.GetFriendRequestByFriendId(requestId);
			if (friendRequest == null)
				return BadRequest(new { result = "request_id_invalid" });

			if (friendRequest.IsFriend)
				return BadRequest(new { result = "friend_request_already_accepted" });

			friendRequest.IsFriend = true;
			friendRequest.FriendshipDate = DateTime.UtcNow;

			_friendRepository.UpdateFriend(friendRequest);

			Notification? notification = _notificationRepository.GetNotificationByContextIdAndNotificationType(requestId, "add-friend-request");

			if (notification == null)
				return NotFound(new { result = "notification_not_found" });

			_notificationRepository.RemoveNotification(notification);

			Notification friendNotif = new Notification
			{
				NotificationType = "accept-friend-request",
				NotifiedUserId = friendRequest.SenderId,
				NotifiedUser = friendRequest.Sender,
				ContextId = friendRequest.Id,
				IsRead = false
			};

			_notificationRepository.AddNotification(friendNotif);

			return Ok(new { result = "friend_request_accepted" });
		}

		[HttpDelete("reject-friend/{requestId}")]
		public IActionResult RejectFriendRequest(Guid requestId)
		{
			string? token = Request.Headers["Authorization"];
			if (token == null || _userRepository.GetUserByToken(token) == null)
				return BadRequest(new { result = "no_token_sent" });

			Friend? friendRequest = _friendRepository.GetFriendRequestByFriendId(requestId);
			if (friendRequest == null)
				return BadRequest(new { result = "friend_request_id_invalid" });

			_friendRepository.RemoveFriend(friendRequest);

			Notification? notification = _notificationRepository.GetNotificationByContextIdAndNotificationType(requestId, "add-friend-request");

			if (notification == null)
				return NotFound(new { result = "notification_not_found" });

			_notificationRepository.RemoveNotification(notification);

			return Ok(new { result = "friend_request_rejected" });
		}

		[HttpDelete("remove-friend/{removeFriendId}")]
		public IActionResult RemoveFriend(Guid removeFriendId)
		{
			string? token = Request.Headers["Authorization"];
			if (token == null || _userRepository.GetUserByToken(token) == null)
				return BadRequest(new { result = "no_token_sent" });

			User? currentUser = _userRepository.GetUserByToken(token);
			if (currentUser == null)
				return BadRequest(new { result = "no_user_found" });

			User? userToRemove = _userRepository.GetUserById(removeFriendId);
			if(userToRemove == null)
				return BadRequest(new { result = "no_user_to_remove_found" });

			Friend? friend = _friendRepository.GetFriendByCurrentUserAndUserToRemoveId(currentUser.Id, userToRemove.Id);
			if (friend == null)
				return BadRequest(new { result = "no_friend_relationship_found" });

			if (!friend.IsFriend)
				return BadRequest(new { result = "not_friend" });

			_friendRepository.RemoveFriend(friend);

			return Ok(new { result = "friend_removed" });
		}

		[HttpGet("get-all-friends/{userId}")]
		public ActionResult<IEnumerable<User>> GetAllFriendsByUserId(Guid userId)
		{
			string? token = Request.Headers["Authorization"];
			if (token == null || _userRepository.GetUserByToken(token) == null)
				return BadRequest(new { result = "no_token_sent" });

			User? user = _userRepository.GetUserById(userId);
			if (user == null)
				user = _userRepository.GetUserByToken(token);

			if (user == null)
				return BadRequest(new { result = "user_not_found" });

			List<User> friends = _friendRepository.GetUserFriendListByUserId(user.Id);

			if (friends == null)
				return BadRequest(new { result = "no_friends_found" });


			return Ok(friends);
		}

		[HttpGet("get-all-friend-request")]
		public ActionResult<IEnumerable<Friend>> GetAllFriendRequests()
		{
			string? token = Request.Headers["Authorization"];
			if (token == null || _userRepository.GetUserByToken(token) == null)
				return BadRequest(new { result = "no_token_sent" });

			User? checkingForUser = _userRepository.GetUserByToken(token);
			if (checkingForUser == null)
				return BadRequest(new { result = "user_not_found" });

			List<Friend> friendRequests = _friendRepository.GetFriendRequestList(checkingForUser.Id);

			if (friendRequests == null)
				return BadRequest(new { result = "no_friend_requests_found" });

			return Ok(friendRequests);
		}

		[HttpPost("get-friend-exist")]
		public ActionResult<Friend?> getFriendExist([FromBody] FriendDTO friendDTO)
		{
			string? token = Request.Headers["Authorization"];
			if (token == null || _userRepository.GetUserByToken(token) == null)
				return BadRequest(new { result = "no_valid_token_sent" });

			User? receiver = _userRepository.GetUserById(friendDTO.ReceiverId);
			if (receiver == null)
				return BadRequest(new { result = "receiver_not_valid" });

			User? sender = _userRepository.GetUserByToken(token);
			if (sender == null)
				return BadRequest(new { result = "sender_not_valid" });

			return _friendRepository.FriendExist(friendDTO.SenderId, friendDTO.ReceiverId);
		}


        [HttpGet("is-poster-friend/{userId}")]
        public ActionResult<bool> IsPosterFriend(Guid userId)
        {
            string? token = Request.Headers["Authorization"];
            if (token == null || _userRepository.GetUserByToken(token) == null)
                return BadRequest(new { result = "no_valid_token_sent" });

            User? user = _userRepository.GetUserByToken(token);
            if (user == null)
            {
                return BadRequest(new { result = "no_vali_token_sent" });
            }

            Friend? friend = _friendRepository.FriendExist(user.Id, userId);
            if (friend == null)
                return BadRequest(new { result = "friend_not_found" });

            return friend.IsFriend;
        }

    }
}
