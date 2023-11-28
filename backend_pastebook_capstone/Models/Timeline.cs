namespace backend_pastebook_capstone.Models
{
	public class Timeline
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public User? User { get; set; }
	}
}
