using backend_pastebook_capstone.Data;
using backend_pastebook_capstone.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_pastebook_capstone.Repository
{
	public class CommentRepository
	{
		private readonly CapstoneDBContext _context;

		public CommentRepository(CapstoneDBContext context)
		{
			_context = context;
		}

		public List<Comment> GetCommentListByPostId(Guid postId)
		{
			return _context.Comment.Where(c => c.PostId == postId).ToList();
		}

		public Comment? GetCommentById(Guid? id)
		{
			return _context.Comment.Include(x => x.Commenter).FirstOrDefault(x => x.Id == id);
		}

		public void AddComment(Comment comment)
		{
			_context.Comment.Add(comment);
			_context.SaveChanges();
		}

		public void UpdateComment(Comment comment)
		{
			_context.Comment.Update(comment);
			_context.SaveChanges();
		}

		public void RemoveComment(Comment comment)
		{
			_context.Comment.Remove(comment);
			_context.SaveChanges();
		}
	}
}