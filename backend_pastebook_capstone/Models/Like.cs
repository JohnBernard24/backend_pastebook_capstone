using System.ComponentModel.DataAnnotations;

namespace backend_pastebook_capstone.Models
{
	public class Like
	{
		public Guid Id { get; set; }
		public Guid PostId { get; set; }
		public Post? Post { get; set; }
		public Guid LikerId { get; set; }
		public User? Liker { get; set; }
	}

	public class LikeDTO
	{
		[Required]
		public Guid PostId { get; set; }
		[Required]
		public Guid LikerId { get; set; }
	}
}
