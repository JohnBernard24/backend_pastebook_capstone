using backend_pastebook_capstone.Models;
using backend_pastebook_capstone.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend_pastebook_capstone.Controllers
{
	[Route("api/post")]
	[ApiController]
	public class PostController : ControllerBase
	{
		private readonly UserRepository _userRepository;
		private readonly PostRepository _postRepository;
		private readonly TimelineRepository _timelineRepository;
		private readonly NotificationRepository _notificationRepository;
		private readonly PhotoRepository _photoRepository;

		public PostController(UserRepository userRepository, PostRepository postRepository, TimelineRepository timelineRepository, NotificationRepository notificationRepository, PhotoRepository photoRepository)
		{
			_userRepository = userRepository;
			_postRepository = postRepository;
			_timelineRepository = timelineRepository;
			_notificationRepository = notificationRepository;
			_photoRepository = photoRepository;
		}

		[HttpPost("add-post")]
		public IActionResult AddPost([FromBody] PostDTO postDTO)
		{
			string? token = Request.Headers["Authorization"];

			if (token == null || _userRepository.GetUserByToken(token) == null)
			{
				return BadRequest(new { result = "no_valid_token_sent" });
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(new { result = "invalid_post" });
			}

			//This gets the poster via its token bc the one logged in is the one who posted
			User? poster = _userRepository.GetUserByToken(token);
			if(poster == null)
			{
				return BadRequest(new { result = "invalid_user_id" });
			}


			Timeline? timeline;
			if (postDTO.UserId == null)
			{
				User? userToPost = _userRepository.GetUserByToken(token);
				timeline = _timelineRepository.GetTimelineByUserId(userToPost?.Id);
			}
			else
			{
				timeline = _timelineRepository.GetTimelineByUserId(postDTO.UserId);
			}

			if (timeline == null)
				return BadRequest(new { result = "user_not_found" });

			Photo? photo = _photoRepository.GetPhotoByPhotoId(postDTO.PhotoId);
			Post post = new Post
			{
				PostTitle = postDTO.PostTitle,
				PostBody = postDTO.PostBody,
				TimelineId = timeline.Id,
				Timeline = timeline,
				PhotoId = postDTO.PhotoId,
				Photo = photo,
				PosterId = poster.Id,
				Poster = poster
			};

			_postRepository.AddPost(post);


			return Ok(new {result = "post_added_successfully"});
		}

		[HttpPut("update-post")]
		public IActionResult UpdatePost([FromBody] PostDTO postDTO)
		{
			string? token = Request.Headers["Authorization"];

			if (token == null || _userRepository.GetUserByToken(token) == null)
			{
				return BadRequest(new { result = "no_valid_token_sent" });
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(new { result = "invalid_post" });
			}

			Post? existingPost = _postRepository.GetPostByPostId(postDTO.Id);
			if(existingPost == null)
			{
				return BadRequest(new { result = "post_not_found" });
			}

			existingPost.PostTitle = postDTO.PostTitle;
			existingPost.PostBody = postDTO.PostBody;
			Photo? photo = _photoRepository.GetPhotoByPhotoId(postDTO.PhotoId);
			existingPost.Photo = photo;

			_postRepository.UpdatePost(existingPost);

			return Ok(existingPost.Id);
		}

		[HttpDelete("delete-post/{postId}")]
		public IActionResult DeletePost(Guid postId)
		{
			string? token = Request.Headers["Authorization"];

			if (token == null || _userRepository.GetUserByToken(token) == null)
			{
				return BadRequest(new { result = "no_valid_token_sent" });
			}

			Post? existingPost = _postRepository.GetPostByPostId(postId);

			if (existingPost == null)
			{
				return BadRequest(new { result = "post_not_found" });
			}

			_postRepository.RemovePost(existingPost);

			return Ok(new { result = "post_deleted" });
		}



		[HttpGet("get-post/{postId}")]
		public IActionResult GetPostByPostId(Guid postId)
		{
			string? token = Request.Headers["Authorization"];

			if (token == null || _userRepository.GetUserByToken(token) == null)
			{
				return BadRequest(new { result = "no_valid_token_sent" });
			}

			Post? post = _postRepository.GetPostByPostId(postId);
			if (post == null)
			{
				return BadRequest(new { result = "post_not_found" });
			}
		
			PostDTO postDTO = new PostDTO
			{
				Id = post.Id,
				PostTitle = post.PostTitle,
				PostBody = post.PostBody,
				DatePosted = post.DatePosted,
				PhotoId = post.PhotoId,
				UserId = _userRepository.GetUserByTimelineId(post.TimelineId)?.Id,
				PosterId = post.PosterId,
			};

			return Ok(postDTO);
		}

		[HttpGet("get-post-likes/{postId}")]
		public ActionResult<IEnumerable<MiniProfileDTO>> GetPostLikesByPostId(Guid postId)
		{
			string? token = Request.Headers["Authorization"];

			if (token == null || _userRepository.GetUserByToken(token) == null)
			{
				return BadRequest(new { result = "no_valid_token_sent" });
			}
			List<Like> likes = _postRepository.GetLikeListByPostId(postId);

			if(likes == null)
			{
				return BadRequest(new { result = "no_post_likes_found" });
			}

			List<MiniProfileDTO?> users = new List<MiniProfileDTO?>();
			
			foreach(Like like in likes)
			{
				User? user = _userRepository.GetUserById(like.LikerId);
				if(user == null)
				{
					continue;
				}

				MiniProfileDTO MiniProfileDTO = new MiniProfileDTO
				{
					Id = user.Id,
					FirstName = user.FirstName,
					LastName = user.LastName,
					Photo = user.Photo
				};

				users.Add(MiniProfileDTO);
			} 

			return Ok(users);
		}

		[HttpPost("like-post")]
		public IActionResult LikePost([FromBody] LikeDTO likeDTO)
		{
			string? token = Request.Headers["Authorization"];
			if (token == null || _userRepository.GetUserByToken(token) == null)
				return BadRequest(new { result = "no_valid_token_sent" });

			if (!ModelState.IsValid)
				return BadRequest(new { result = "invalid_like_to_post" });

			Post? post = _postRepository.GetPostByPostId(likeDTO.PostId);
			if (post == null)
				return NotFound(new { result = "post_not_found" });

			User? liker = _userRepository.GetUserById(likeDTO.LikerId);

			if (liker == null)
				return NotFound(new { result = "liker_not_found" });

			Like? existingLike = _postRepository.GetLikeByPostIdAndUserId(likeDTO.PostId, likeDTO.LikerId);

			if (existingLike == null)
			{
				Like like = new Like
				{
					PostId = likeDTO.PostId,
					Post = post,
					LikerId = likeDTO.LikerId,
					Liker = liker
				};

				_postRepository.AddLike(like);

				var likeNotif = new Notification
				{
					NotificationType = "like",
					NotifiedUserId = post.PosterId,
					NotifiedUser = post.Poster,
					ContextId = like.Id
				};

				_notificationRepository.AddNotification(likeNotif);

				return Ok(new { result = "like_added" });
			}

			_postRepository.RemoveLike(existingLike);
			return Ok(new { result = "like_deleted" });

		}

		[HttpGet("is-post-liked/{postId}")]
		public ActionResult<bool> IsPostLiked(Guid postId)
		{
			string? token = Request.Headers["Authorization"];
			if (token == null || _userRepository.GetUserByToken(token) == null)
				return BadRequest(new { result = "no_valid_token_sent" });

			User? user = _userRepository.GetUserByToken(token);
			if(user == null)
			{
				return BadRequest(new { result = "no_vali_token_sent" });
			}

			Post? post = _postRepository.GetPostByPostId(postId);
			if (post == null)
				return NotFound(new { result = "post_not_found" });

			var isLiked = _postRepository.IsCurrentPostLiked(postId, user.Id);

			return isLiked;
		}
	}
}
