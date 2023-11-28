using backend_pastebook_capstone.AuthenticationService.Models;
using backend_pastebook_capstone.Data;

namespace backend_pastebook_capstone.AuthenticationService.Repository
{
	public class AccessTokenRepository
	{
		private readonly CapstoneDBContext _context;


		public AccessTokenRepository(CapstoneDBContext context)
		{
			_context = context;
		}

		public void InsertToken(AccessToken token)
		{
			_context.AccessToken.Add(token);
			_context.SaveChanges();
		}

		public void UpdateToken(AccessToken token)
		{
			_context.AccessToken.Update(token);
			_context.SaveChanges();
		}
		public void DeleteToken(AccessToken token)
		{
			_context.AccessToken.Remove(token);
			_context.SaveChanges();
		}


		public void DeleteAllToken(Guid userId)
		{
			IEnumerable<AccessToken> tokens = _context.AccessToken.ToArray().Where(t => t.UserId == userId).ToList();

			_context.AccessToken.RemoveRange(tokens);
			_context.SaveChanges();
		}
	}
}
