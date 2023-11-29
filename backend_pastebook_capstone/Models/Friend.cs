using System.ComponentModel.DataAnnotations;

namespace backend_pastebook_capstone.Models
{
	public class Friend
	{
		public Guid Id { get; set; }
		public Guid ReceiverId { get; set; }
		public User? Receiver { get; set; }
		public Guid SenderId { get; set; }
		public User? Sender { get; set; }
		public DateTime? FriendshipDate { get; set; }
		public bool IsFriend { get; set; } = false;
	}

	public class FriendDTO
	{
		public Guid? Id { get; set; }
		public Guid ReceiverId { get; set; }
		public Guid SenderId { get; set; }
		public DateTime? FriendshipDate { get; set; } = DateTime.Now;
	}
}
