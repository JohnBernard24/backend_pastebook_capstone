using backend_pastebook_capstone.Models;
using backend_pastebook_capstone.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend_pastebook_capstone.Controllers
{
	[Route("api/comment")]
	[ApiController]
	public class CommentController : ControllerBase
	{
		private readonly PostRepository _postRepository;
		private readonly CommentRepository _commentRepository;
		private readonly NotificationRepository _notificationRepository;
		private readonly UserRepository _userRepository;

		public CommentController(PostRepository postRepository, CommentRepository commentRepository, NotificationRepository notificationRepository, UserRepository userRepository)
		{
			_postRepository = postRepository;
			_commentRepository = commentRepository;
			_notificationRepository = notificationRepository;
			_userRepository = userRepository;
		}

		[HttpPost("add-comment")]
		public IActionResult AddComment([FromBody] CommentDTO commentDTO)
		{
			string? token = Request.Headers["Authorization"];
			if (token == null)
				return BadRequest(new { result = "no_valid_token_sent" });

			if (!ModelState.IsValid)
				return BadRequest(new { result = "invalid_comment" });

			Post? post = _postRepository.GetPostByPostId(commentDTO.PostId);

			if (post == null)
				return BadRequest(new { result = "invalid_post_id" });

			//This gets the poster via its token bc the one logged in is the one who posted
			User? commenter = _userRepository.GetUserByToken(token);
			if (commenter == null)
				return BadRequest(new { result = "no_valid_token_sent" });

			Comment comment = new Comment
			{
				CommentContent = commentDTO.CommentContent,
				PostId = commentDTO.PostId,
				Post = post,
				CommenterId = commenter.Id,
				Commenter = commenter
			};

			_commentRepository.AddComment(comment);

			Notification commentNotif = new Notification
			{
				NotificationType = "comment",
				NotifiedUserId = comment.Post.PosterId,
				NotifiedUser = comment.Post.Poster,
				ContextId = comment.Id
			};

			_notificationRepository.AddNotification(commentNotif);

			return Ok(comment.Id);
		}


		[HttpPut("update-comment")]
		public IActionResult UpdateComment([FromBody] CommentDTO commentDTO)
		{
			string? token = Request.Headers["Authorization"];
			if (token == null || _userRepository.GetUserByToken(token) == null)
				return BadRequest(new { result = "no_valid_token_sent" });

			if (!ModelState.IsValid)
				return BadRequest(new { result = "invalid_comment" });

			Comment? existingComment = _commentRepository.GetCommentById(commentDTO.Id);

			if (existingComment == null)
				return BadRequest(new { result = "comment_not_found" });

			existingComment.CommentContent = commentDTO.CommentContent;

			_commentRepository.UpdateComment(existingComment);

			commentDTO.DateCommented = existingComment.DateCommented;
			commentDTO.PostId = existingComment.PostId;
			commentDTO.CommenterId = existingComment.CommenterId;

			return Ok(commentDTO);
		}

		[HttpDelete("delete-comment/{commentId}")]
		public IActionResult DeleteComment(Guid commentId)
		{
			string? token = Request.Headers["Authorization"];
			if (token == null || _userRepository.GetUserByToken(token) == null)
				return BadRequest(new { result = "no_valid_token_sent" });

			Comment? existingComment = _commentRepository.GetCommentById(commentId);
			if (existingComment == null)
				return BadRequest(new { result = "comment_not_found" });

			_commentRepository.RemoveComment(existingComment);

			return Ok(new { result = "comment_deleted" });
		}

		[HttpGet("get-post-comments/{postId}")]
		public IActionResult GetCommentsByPostId(Guid postId)
		{
			string? token = Request.Headers["Authorization"];
			if (token == null || _userRepository.GetUserByToken(token) == null)
				return BadRequest(new { result = "no_valid_token_sent" });

			Post? post = _postRepository.GetPostByPostId(postId);
			if(post == null)
				return NotFound(new { result = "invalid_post_id" });

			List<Comment> commentList = _commentRepository.GetCommentListByPostId(post.Id);
			if(commentList == null)
				return NotFound(new { result = "no_comments_found" });

			List<CommentDTO> commentDTOList = new List<CommentDTO>();
			foreach(Comment comment in commentList)
			{
				commentDTOList.Add(new CommentDTO
				{
					Id = comment.Id,
					CommentContent = comment.CommentContent,
					DateCommented = comment.DateCommented,
					PostId = comment.PostId,
					CommenterId = comment.CommenterId
				});
			}

			return Ok(commentDTOList);
		}





	}
}
