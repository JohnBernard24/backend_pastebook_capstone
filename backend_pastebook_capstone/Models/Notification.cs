namespace backend_pastebook_capstone.Models
{
	public class Notification
	{
		public Guid Id { get; set; }
		public Guid NotifiedUserId { get; set; }
		public User? NotifiedUser { get; set; }
		public string NotificationType { get; set; } = null!;

		public DateTime NotifiedDate { get; set; } = DateTime.Now;

		public Guid ContextId { get; set; }
		public bool IsRead { get; set; }
	}

	
}
