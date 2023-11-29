using backend_pastebook_capstone.Models;
using backend_pastebook_capstone.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend_pastebook_capstone.Controllers
{
	[Route("api/timeline")]
	[ApiController]
	public class TimelineController : ControllerBase
	{
		private readonly TimelineRepository _timelineRepository;
		private readonly FriendRepository _friendRepository;
		private readonly PostRepository _postRepository;
		private readonly UserRepository _userRepository;

		public TimelineController(TimelineRepository timelineRepository, FriendRepository friendRepository, PostRepository postRepository, UserRepository userRepository)
		{
			_timelineRepository = timelineRepository;
			_friendRepository = friendRepository;
			_postRepository = postRepository;
			_userRepository = userRepository;
		}

		[HttpGet("get-all-posts")]
		public ActionResult<IEnumerable<PostDTO>> GetAllPosts()
		{
			string? token = Request.Headers["Authorization"];
			if (token == null)
				return BadRequest(new { result = "no_valid_token_sent" });

			User? user = _userRepository.GetUserByToken(token);
			if (user == null)
				return BadRequest(new { result = "no_valid_token_sent" });


			Timeline? timeline = _timelineRepository.GetTimelineByUserId(user.Id);
			if (timeline == null)
				return BadRequest(new { result = "user_invalid" });

			List<Post>? posts = _timelineRepository.GetPostListByTimelineId(timeline.Id);

			if (posts == null)
			{
				return NotFound(new { result = "no_posts_found" });
			}

			List<PostDTO>? postDTOs = new List<PostDTO>();
			foreach (var post in posts)
			{
				var postDTO = new PostDTO
				{
					Id = post.Id,
					PostTitle = post.PostTitle,
					PostBody = post.PostBody,
					DatePosted = post.DatePosted,
					PhotoId = post.PhotoId,
					UserId = _userRepository.GetUserByTimelineId(timeline.Id)?.Id,
					PosterId = post.PosterId
				};

				postDTOs.Add(postDTO);
			}

			return postDTOs;
		}

		[HttpGet("get-newsfeed-posts")]
		public ActionResult<IEnumerable<PostDTO>> GetAllNewsfeedPostsByUserId()
		{
			string? token = Request.Headers["Authorization"];
			if (token == null)
				return BadRequest(new { result = "no_valid_token_sent" });

			User? user = _userRepository.GetUserByToken(token);
			if (user == null)
				return BadRequest(new { result = "no_valid_token_sent" });

			List<Friend> friends = _friendRepository.GetFriendListByUserId(user.Id);

			if (friends == null)
			{
				return BadRequest(new { result = "no_friends_found" });
			}

			List<Post> friendsPosts = new List<Post>();

			foreach (Friend friend in friends)
			{
				List<Post> individualPost = _postRepository.GetPostListByUserId(friend.SenderId);

				foreach (Post post in individualPost)
				{
					friendsPosts.Add(post);
				}
			}

			List<Post> usersPosts = _postRepository.GetPostListByUserId(user.Id);

			List<Post> allPosts = friendsPosts.Concat(usersPosts).ToList();

			allPosts = allPosts.OrderByDescending(post => post.DatePosted).ToList();


			List<PostDTO>? postDTOs = new List<PostDTO>();
			foreach (var post in allPosts)
			{
				var postDTO = new PostDTO
				{
					Id = post.Id,
					PostTitle = post.PostTitle,
					PostBody = post.PostBody,
					DatePosted = post.DatePosted,
					PhotoId = post.PhotoId,
					UserId = _userRepository.GetUserByTimelineId(post.TimelineId)?.Id,
					PosterId = post.PosterId
				};

				postDTOs.Add(postDTO);
			}

			return postDTOs;
		}
	}
}
