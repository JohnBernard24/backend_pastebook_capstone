namespace backend_pastebook_capstone.AuthenticationService.Models
{
	public class AccessToken
	{
		public Guid Id { get; set; }
		public string? Token { get; set; }
		public Guid UserId { get; set; }
	}
}
