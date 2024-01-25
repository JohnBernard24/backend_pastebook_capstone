using System.ComponentModel.DataAnnotations;

namespace backend_pastebook_capstone.Models
{
	public class Post
	{
		public Guid Id { get; set; }
		public string PostTitle { get; set; } = null!;
		public string PostBody { get; set; } = null!;
		public DateTime DatePosted { get; set; } = DateTime.Now;

		public Guid TimelineId { get; set; }
		public Timeline? Timeline { get; set; }
		public Guid? PhotoId { get; set; }
		public Photo? Photo { get; set; }
		public Guid PosterId { get; set; }
		public User? Poster { get; set; }
	}

	public class PostDTO
	{
		public Guid? Id { get; set; }

		[Required]
		public string PostTitle { get; set; } = null!;

		[Required]
		public string PostBody { get; set; } = null!;
		public DateTime DatePosted { get; set; } = DateTime.Now;

		public Guid? PhotoId { get; set; }
		public Guid? UserId { get; set; }
	}

	

}
