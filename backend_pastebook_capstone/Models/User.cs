using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_pastebook_capstone.Models
{
	public class User
	{
		public Guid Id { get; set; }
		public string FirstName { get; set; } = null!;
		public string LastName { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string HashedPassword { get; set; } = null!;

		[Column(TypeName = "DATE")]
		public DateTime BirthDate { get; set; }

		public string? Sex { get; set; }
		public string? PhoneNumber { get; set; }
		public string? AboutMe { get; set; }


		public Guid? ProfileImageId { get; set; }
		public Photo? Photo { get; set; }
	}

	public class UserLoginDTO
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; } = null!;
		[Required]
		public string Password { get; set; } = null!;
	}

	public class LoginResponse
	{
		public Guid UserId { get; set; }
		public string? Email { get; set; }
		public string? Token { get; set; }
	}

	public class UserRegisterDTO
	{
		[Required]
		public string FirstName { get; set; } = null!;
		[Required]
		public string LastName { get; set; } = null!;

		[Required]
		[EmailAddress]
		public string Email { get; set; } = null!;
		[Required]
		public string Password { get; set; } = null!;
		[Required]
		public DateTime BirthDate { get; set; }

		public string? Sex { get; set; }
		public string? PhoneNumber { get; set; }

	}

	public class MiniProfileDTO
	{
		public Guid? Id { get; set; }
		[Required]
		public string FirstName { get; set; } = null!;
		[Required]
		public string LastName { get; set; } = null!;
		public Photo? Photo { get; set; }
		public int? FriendCount { get; set; }
	}

	public class ProfileDTO
	{
		public Guid? Id { get; set; }
		[Required]
		public string FirstName { get; set; } = null!;
		[Required]
		public string LastName { get; set; } = null!;
		[Required]
		public DateTime BirthDate { get; set; }

		public string? Sex { get; set; }

		public string? PhoneNumber { get; set; }

		public string? AboutMe { get; set; }
	}

	public class EditPasswordDTO
	{
		public string CurrentPassword { get; set; } = null!;
		public string NewPassword { get; set; } = null!;
	}

    public class ForgotPasswordDTO
    {
		public string Email { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }


}
