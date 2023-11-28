namespace backend_pastebook_capstone.Models
{
	public class Friend
	{
		public Guid Id { get; set; }
		public Guid RecieverId { get; set; }
		public User? Receiever { get; set; }
		public Guid SenderId { get; set; }
		public User? Sender { get; set; }
		public DateTime? FriendshipDate { get; set; }
		public bool IsFriend { get; set; } = false;
	}


}
