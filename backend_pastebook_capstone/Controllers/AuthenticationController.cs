using backend_pastebook_capstone.AuthenticationService.Authenticator;
using backend_pastebook_capstone.AuthenticationService.Repository;
using backend_pastebook_capstone.Models;
using backend_pastebook_capstone.Repository;
using backend_pastebook_capstone.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg;
using System.Net.Mail;
using System.Net;

namespace backend_pastebook_capstone.Controllers
{
	[Route("api/authentication")]
	[ApiController]
	public class AuthenticationController : ControllerBase
	{
		private readonly BcryptPasswordHasher _passwordHasher;
		private readonly Authenticator _authenticator;
		private readonly AccessTokenRepository _accessTokenRepository;


		private readonly UserRepository _userRepository;
		private readonly TimelineRepository _timelineRepository;
		private readonly AlbumRepository _albumRepository;
		private readonly VerificationRepository _verificationRepository;

		public AuthenticationController(UserRepository userRepository, BcryptPasswordHasher passwordHasher, Authenticator authenticator, TimelineRepository timelineRepository, AlbumRepository albumRepository, AccessTokenRepository accessTokenRepository, VerificationRepository verificationRepository)
		{
			_passwordHasher = passwordHasher;
			_authenticator = authenticator;

			_userRepository = userRepository;
			_timelineRepository = timelineRepository;
			_albumRepository = albumRepository;
			_accessTokenRepository = accessTokenRepository;
			_verificationRepository = verificationRepository;
		}


		[HttpPost("login")]
		public IActionResult Login([FromBody] UserLoginDTO userLoginDTO)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new { result = "invalid_user_login" });
			}

			User? user = _userRepository.GetUserByEmail(userLoginDTO.Email);
			if (user == null)
			{
				return Unauthorized(new { result = "no_user_found" });
			}

			bool isCorrectPassword = _passwordHasher.VerifyPassword(userLoginDTO.Password, user.HashedPassword);
			if (!isCorrectPassword)
			{
				return Unauthorized(new { result = "invalid_credentials" });
			}

			string token = _authenticator.Authenticate(user);

			LoginResponse loginResponse = new LoginResponse
			{
				UserId = user.Id,
				Email = user.Email,
				Token = token
			};


			return Ok(loginResponse);
		}

		[HttpPost("register")]
		public IActionResult Register([FromBody] UserRegisterDTO userRegisterDTO)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new { result = "invalid_user_registration" });
			}

			User? existingUser = _userRepository.GetUserByEmail(userRegisterDTO.Email);
			if (existingUser != null)
			{
				return Conflict(new { result = "email_already_exits" });
			}

			User user = new User
			{
				FirstName = userRegisterDTO.FirstName,
				LastName = userRegisterDTO.LastName,
				Email = userRegisterDTO.Email,
				HashedPassword = _passwordHasher.HashPassword(userRegisterDTO.Password),
				BirthDate = userRegisterDTO.BirthDate,
				Sex = userRegisterDTO.Sex,
				PhoneNumber = userRegisterDTO.PhoneNumber
			};

			Timeline timeline = new Timeline
			{
				UserId = user.Id,
				User = user
			};

			Album album = new Album
			{
				AlbumName = "Uploads",
				UserId = user.Id,
				User = user
			};

			_userRepository.AddUser(user);
			_timelineRepository.AddTimeline(timeline);
			_albumRepository.AddAlbum(album);

			return Ok(new { result = "user_registered_successfully" });
		}

		[HttpPost("logout")]
		public IActionResult logout()
		{
			string? token = Request.Headers["Authorization"];
			if (token == null)
			{
				return BadRequest(new { result = "no_valid_token_sent" });
			}
			User? user = _userRepository.GetUserByToken(token);
			if (user == null)
			{
				return BadRequest(new { result = "no_user_found" });
			}

			_accessTokenRepository.DeleteAllToken(user.Id);
			return Ok(new { result = "logout successfully" });
		}

        [HttpGet("validate-token")]
        public ActionResult<bool> Validate()
        {
            string? token = Request.Headers["Authorization"];

            if (token == null)
            {
                return BadRequest(new { result = "no_valid_token_sent" });
            }

			User? user = _userRepository.GetUserByToken(token);

			if (user == null)
			{
				return Unauthorized(new { result = "no_user_found" });
			}

			bool isValid = _authenticator.Validate(token);

            if (isValid)
            {
                return Ok(true);
            }
            else
            {
                return Unauthorized(new { result = "invalid_token" });
            }
        }


		[HttpPost("verify-email-forgot/{recipientEmail}")]
		public IActionResult SendEmailForgot(string recipientEmail)
		{
			User? user = _userRepository.GetUserByEmail(recipientEmail);
			if (user == null)
				return BadRequest(new { result = "no_account_with_that_email" });

			bool result = _verificationRepository.SendVerificationEmail(recipientEmail);

			if (result)
			{
				return Ok(new { result = "Email sent successfully!" });

			}
			else
			{
				return BadRequest(new { result = "Error sending email." });
            }
		}

        [HttpPost("verify-email-new-user/{recipientEmail}")]
        public IActionResult SendNewUserEmail(string recipientEmail)
        {
            bool result = _verificationRepository.SendVerificationEmail(recipientEmail);

            if (result)
            {
				return Ok(new { result = "Email sent successfully!" });
				//return true;
            }
            else
            {
				return BadRequest(new { result = "Error sending email." });
				//return false;
            }
        }
        

        [HttpPost("verify-code")]	
		public ActionResult<bool> VerifyCode([FromBody] VerificationDTO verificationDTO)
		{
			Verification? verification = _verificationRepository.GetVerificationByEmail(verificationDTO.Email);
			if(verification == null)
			{
				return BadRequest(new { result = "no_verification_with_that_email" });
			}

			return verificationDTO.VerificationCode == verification.VerificationCode;
		}

		[HttpPost("check-email-availability/{email}")]
		public ActionResult<bool> CheckEmail(string email)
		{
			User? user = _userRepository.GetUserByEmail(email);
			if (user == null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
