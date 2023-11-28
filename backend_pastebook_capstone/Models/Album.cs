namespace backend_pastebook_capstone.Models
{
	public class Album
	{
		public Guid Id { get; set; }
		public string AlbumName { get; set; } = null!;
		public Guid UserId { get; set; }
		public User? User { get; set; } = null!;
	}
}
