using backend_pastebook_capstone.AuthenticationService.Models;
using backend_pastebook_capstone.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_pastebook_capstone.Data
{
	public class CapstoneDBContext : DbContext
	{
		public DbSet<User> User { get; set; } = default!;
		public DbSet<Timeline> TimeLine { get; set; } = default!;
		public DbSet<Post> Post { get; set; } = default!;
		public DbSet<Photo> Photo { get; set; } = default!;
		public DbSet<Like> Like { get; set; } = default!;
		public DbSet<Comment> Comment { get; set; } = default!;
		public DbSet<Friend> Friend { get; set; } = default!;
		public DbSet<Album> Album { get; set; } = default!;
		public DbSet<Notification> Notification { get; set; } = default!;
		public DbSet<AccessToken> AccessToken { get; set; } = default!;


		public CapstoneDBContext(DbContextOptions<CapstoneDBContext> options)
			: base(options)
		{
		}
	}
}
