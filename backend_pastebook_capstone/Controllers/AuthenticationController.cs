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
		private readonly PhotoRepository _photoRepository;

		public AuthenticationController(UserRepository userRepository, BcryptPasswordHasher passwordHasher, Authenticator authenticator, TimelineRepository timelineRepository, AlbumRepository albumRepository, AccessTokenRepository accessTokenRepository, VerificationRepository verificationRepository, PhotoRepository photoRepository)
		{
			_passwordHasher = passwordHasher;
			_authenticator = authenticator;

			_userRepository = userRepository;
			_timelineRepository = timelineRepository;
			_albumRepository = albumRepository;
			_accessTokenRepository = accessTokenRepository;
			_verificationRepository = verificationRepository;
			_photoRepository = photoRepository;
		}


		[HttpPost("login")]
		public IActionResult Login([FromBody] UserLoginDTO userLoginDTO)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new { result = "Invalid user login" });
			}

			User? user = _userRepository.GetUserByEmail(userLoginDTO.Email);
			if (user == null)
			{
				return Unauthorized(new { result = "Could not find account!" });
			}

			bool isCorrectPassword = _passwordHasher.VerifyPassword(userLoginDTO.Password, user.HashedPassword);
			if (!isCorrectPassword)
			{
				return Unauthorized(new { result = "Invalid credentials!" });
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
                return BadRequest(new { result = "Invalid user registration" });
            }

            User? existingUser = _userRepository.GetUserByEmail(userRegisterDTO.Email);
            if (existingUser != null)
            {
                return Conflict(new { result = "Email Already Exists!" });
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

            string fileName;
            if (userRegisterDTO.Sex == "Male")
            {
                fileName = "sample_avatar_male.png";
            }
            else if (userRegisterDTO.Sex == "Female")
            {
                fileName = "sample_avatar_female.png";
            }
            else
            {
                fileName = "sample_avatar_neutral.png";
            }

            string wwwrootPath = "wwwroot";
            string sourceFilePath = Path.Combine(wwwrootPath, fileName);

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;
            string uploadsFolder = Path.Combine("..", "..", "PastebookData", "photos", album.Id.ToString());
            string destinationFilePath = Path.Combine(uploadsFolder, uniqueFileName);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            System.IO.File.Copy(sourceFilePath, destinationFilePath, true);

            string imageUrl = $"photos/{album.Id}/{uniqueFileName}";

            Photo photo = new Photo
            {
                PhotoImageURL = imageUrl,
                AlbumId = album.Id,
                Album = album
            };

            user.PhotoId = photo.Id;
            user.Photo = photo;

            _photoRepository.AddPhoto(photo);

            return Ok(new { result = "User Registered Successfully" });
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
				return BadRequest(new { result = "Account does not exist" });

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
            }
            else
            {
				return BadRequest(new { result = "Error sending email." });
            }
        }
        

        [HttpPost("verify-code")]	
		public ActionResult<bool> VerifyCode([FromBody] VerificationDTO verificationDTO)
		{
			Verification? verification = _verificationRepository.GetVerificationByEmail(verificationDTO.Email);
			if(verification == null)
			{
				return BadRequest(new { result = "No Verification with that email" });
			}
            bool isVerified = verificationDTO.VerificationCode == verification.VerificationCode;

            _verificationRepository.RemoveVerification(verification);

            return isVerified;
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

        [HttpPut("forgot-change-password")]
        public IActionResult EditPassword([FromBody] ForgotPasswordDTO forgotPasswordDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { result = "invalid_user" });

            User? existingUser = _userRepository.GetUserByEmail(forgotPasswordDTO.Email);
            if (existingUser == null)
                return NotFound(new { result = "user_not_found" });


            existingUser.HashedPassword = _passwordHasher.HashPassword(forgotPasswordDTO.NewPassword);

            _userRepository.UpdateUser(existingUser);

            return Ok(new {result = "password_changed_successfully"});
        }
    }
}
