using backend_pastebook_capstone.AuthenticationService.Models;
using backend_pastebook_capstone.Data;
using backend_pastebook_capstone.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_pastebook_capstone.Repository
{
	public class UserRepository
	{
		public readonly CapstoneDBContext _context;
		public readonly TimelineRepository _timelineRepository;

		public UserRepository(CapstoneDBContext context, TimelineRepository timelineRepository)
		{
			_context = context;
			_timelineRepository = timelineRepository;
		}

		public User? GetUserById(Guid? id)
		{
			return _context.User.FirstOrDefault(x => x.Id == id);
		}
		public User? GetUserByEmail(string email)
		{
			return _context.User.FirstOrDefault(u => u.Email == email);
		}
		public User? GetUserByToken(string token)
		{
			AccessToken? accessToken = _context.AccessToken.FirstOrDefault(t => t.Token == token);

			if (accessToken == null)
				return null;

			User? user = _context.User.Include(u => u.Photo).FirstOrDefault(u => u.Id == accessToken.UserId);
			return user;
		}

		public User? GetUserByTimelineId(Guid timelineId)
		{
			Timeline? timeline = _timelineRepository.GetTimelineByTimelineId(timelineId);
			if(timeline == null)
				return null;
			return _context.User.FirstOrDefault(u => u.Id == timeline.UserId);
		}

		public List<User> GetUserListBySearchName(string name)
		{
			return _context.User.Where(u => u.FirstName.Contains(name) || u.LastName.Contains(name)).ToList();
		}

		public void AddUser(User user)
		{
			_context.User.Add(user);
			_context.SaveChanges();
		}
		public void UpdateUser(User user)
		{
			_context.User.Update(user);
			_context.SaveChanges();
		}
		public void RemoveUser(User user)
		{
			_context.User.Remove(user);
			_context.SaveChanges();
		}
	}
}
