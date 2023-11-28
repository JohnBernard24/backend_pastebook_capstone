namespace backend_pastebook_capstone.Services
{
	public class BcryptPasswordHasher
	{
		public string HashPassword(string password)
		{
			return BCrypt.Net.BCrypt.HashPassword(password);
		}

		public bool VerifyPassword(string password, string passwordHash)
		{
			return BCrypt.Net.BCrypt.Verify(password, passwordHash);
		}
	}
}
