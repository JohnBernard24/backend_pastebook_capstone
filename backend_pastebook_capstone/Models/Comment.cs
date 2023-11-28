namespace backend_pastebook_capstone.Models
{
	public class Comment
	{
		public Guid Id { get; set; }
		public string CommentContent { get; set; } = null!;
		public DateTime DateCommented { get; set; } = DateTime.Now;

		public Guid PostId{ get; set; }
		public Post? Post { get; set; }
		public Guid CommenterId { get; set; }
		public User? Commenter { get; set; }
	}
}
