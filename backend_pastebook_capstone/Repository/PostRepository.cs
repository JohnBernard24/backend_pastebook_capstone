using backend_pastebook_capstone.Data;
using backend_pastebook_capstone.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_pastebook_capstone.Repository
{
	public class PostRepository
	{
		private readonly CapstoneDBContext _context;

		public PostRepository(CapstoneDBContext context)
		{
			_context = context;
		}

		public Post? GetPostByPostId(Guid? postId)
		{
			return _context.Post.FirstOrDefault(p => p.Id == postId);
		}

		public List<Post> GetPostListByUserId(Guid userId)
		{
			return _context.Post
				.Where(post => post.PosterId == userId)
				.Include(post => post.Poster)
				.Include(post => post.Timeline)
				.Include(post => post.Photo)
				.ToList();
		}


		public Like? GetLikeByLikeId(Guid likeId)
		{
			return _context.Like.FirstOrDefault(l => l.Id == likeId);
		}

		public Like? GetLikeByPostIdAndUserId(Guid postId, Guid likerId)
		{
			return _context.Like.FirstOrDefault(l => l.PostId == postId && l.LikerId == likerId);
		}
		
		public bool IsCurrentPostLiked(Guid postId, Guid likerId)
		{
			return _context.Like.Any(l => l.PostId == postId && l.LikerId == likerId);
		}

		public List<Like> GetLikeListByPostId(Guid postId)
		{
			return _context.Like.Where(l => l.PostId == postId).ToList();
		}


		public void AddPost(Post post)
		{
			_context.Post.Add(post);
			_context.SaveChanges();
		}

		public void UpdatePost(Post post)
		{
			_context.Post.Update(post);
			_context.SaveChanges();
		}

		public void RemovePost(Post post)
		{
			_context.Post.Remove(post);
			_context.SaveChanges();
		}

		public void AddLike(Like like)
		{
			_context.Like.Add(like);
			_context.SaveChanges();
		}

		public void RemoveLike(Like like)
		{
			_context.Like.Remove(like);
			_context.SaveChanges();
		}
	}
}
