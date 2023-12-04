using backend_pastebook_capstone.Data;
using backend_pastebook_capstone.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_pastebook_capstone.Repository
{
	public class TimelineRepository
	{
		private readonly CapstoneDBContext _context;

		public TimelineRepository(CapstoneDBContext context)
		{
			_context = context;
		}

		public Timeline? GetTimelineByUserId(Guid? userId)
		{
			return _context.TimeLine.FirstOrDefault(t => t.UserId == userId);
		}

		public List<Post> GetPostListByTimelineId(Guid timelineId)
		{
			return _context.Post
				.Where(post => post.TimelineId == timelineId)
				.Include(post => post.Poster)
				.Include(post => post.Poster!.Photo)
				.Include(post => post.Timeline)
				.Include(post => post.Photo)
				.ToList();
		}

		public Timeline? GetTimelineByTimelineId(Guid timelineId)
		{
			return _context.TimeLine.FirstOrDefault(t => t.Id == timelineId);
		}

		public void AddTimeline(Timeline timeline)
		{
			_context.TimeLine.Add(timeline);
			_context.SaveChanges();
		}


	}
}
