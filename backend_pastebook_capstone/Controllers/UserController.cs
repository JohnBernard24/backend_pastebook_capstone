using backend_pastebook_capstone.Models;
using backend_pastebook_capstone.Repository;
using backend_pastebook_capstone.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend_pastebook_capstone.Controllers
{
	[Route("api/profile")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly UserRepository _userRepository;
		private readonly BcryptPasswordHasher _passwordHasher;
		private readonly FriendRepository _friendRepository;
		private readonly PhotoRepository _photoRepository;

		public UserController(UserRepository userRepository, BcryptPasswordHasher passwordHasher, FriendRepository friendRepository, PhotoRepository photoRepository)
		{
			_userRepository = userRepository;
			_passwordHasher = passwordHasher;
			_friendRepository = friendRepository;
			_photoRepository = photoRepository;
		}


		[HttpGet("get-profile/{userId}")]
		public IActionResult GetUserProfile(Guid userId)
		{
			string? token = Request.Headers["Authorization"];
			if (token == null || _userRepository.GetUserByToken(token) == null)
				return BadRequest(new { result = "no_token_sent" });

			if (!ModelState.IsValid)
				return BadRequest(new { result = "invalid_user" });


			User? checkingForUser = _userRepository.GetUserById(userId);
			if (checkingForUser == null)
				checkingForUser = _userRepository.GetUserByToken(token);

			if(checkingForUser == null)
				return BadRequest(new { result = "user_not_found" });

			ProfileDTO profileDTO = new ProfileDTO
			{
				Id = checkingForUser.Id,
				FirstName = checkingForUser.FirstName,
				LastName = checkingForUser.LastName,
				BirthDate = checkingForUser.BirthDate,
				Sex = checkingForUser.Sex,
				PhoneNumber = checkingForUser.PhoneNumber,
				AboutMe = checkingForUser.AboutMe
			};

			return Ok(profileDTO);
		}

		[HttpGet("get-mini-profile/{userId}")]
		public IActionResult GetMiniProfile(Guid userId)
		{
			string? token = Request.Headers["Authorization"];
			if (token == null || _userRepository.GetUserByToken(token) == null)
				return BadRequest(new { result = "no_token_sent" });

			if (!ModelState.IsValid)
				return BadRequest(new { result = "invalid_user" });

			User? checkingForUser = _userRepository.GetUserById(userId);
			if (checkingForUser == null)
				checkingForUser = _userRepository.GetUserByToken(token);

			if (checkingForUser == null)
				return BadRequest(new { result = "user_not_found" });

			int friendCount = _friendRepository.GetFriendCountByUserId(checkingForUser.Id);

			var miniProfile = new MiniProfileDTO
			{
				Id = checkingForUser.Id,
				FirstName = checkingForUser.FirstName,
				LastName = checkingForUser.LastName,
				Photo = checkingForUser.Photo,
				FriendCount = friendCount
			};

			return Ok(miniProfile);
		}

		[HttpPut("edit-profile")]
		public IActionResult EditProfile([FromBody] ProfileDTO profileDTO)
		{
			string? token = Request.Headers["Authorization"];
			if (token == null || _userRepository.GetUserByToken(token) == null)
				return BadRequest(new { result = "no_token_sent" });

			if (!ModelState.IsValid)
				return BadRequest(new { result = "invalid_user" });

			User? existingUser = _userRepository.GetUserByToken(token);
			if (existingUser == null)
				return NotFound(new { result = "user_not_found" });

			existingUser.FirstName = profileDTO.FirstName;
			existingUser.LastName = profileDTO.LastName;
			existingUser.BirthDate = profileDTO.BirthDate;
			existingUser.Sex = profileDTO.Sex;
			existingUser.PhoneNumber = profileDTO.PhoneNumber;
			existingUser.AboutMe = profileDTO.AboutMe;

			_userRepository.UpdateUser(existingUser);

			return Ok(profileDTO);
		}

		[HttpPut("edit-email/{email}")]
		public IActionResult EditEmail(string email)
		{
			string? token = Request.Headers["Authorization"];
			if (token == null || _userRepository.GetUserByToken(token) == null)
				return BadRequest(new { result = "No token sent" });

			User? existingUser = _userRepository.GetUserByToken(token);
			if (existingUser == null)
				return NotFound(new { result = "User token not valid" });

			existingUser.Email = email;

			_userRepository.UpdateUser(existingUser);

			ProfileDTO profileDTO = new ProfileDTO
			{
				Id = existingUser.Id,
				FirstName = existingUser.FirstName,
				LastName = existingUser.LastName,
				BirthDate = existingUser.BirthDate,
				Sex = existingUser.Sex,
				PhoneNumber = existingUser.PhoneNumber,
				AboutMe = existingUser.AboutMe
			};

			return Ok(profileDTO);
		}

		[HttpPost("check-password/{currentPassword}")]
		public ActionResult<bool> CheckPassword(string currentPassword)
		{
			string? token = Request.Headers["Authorization"];
			if(token == null)
                return BadRequest(new { result = "Invalid token sent" });

            User? user = _userRepository.GetUserByToken(token);
            if (user == null)
                return NotFound(new { result = "No user found" });

			if (!_passwordHasher.VerifyPassword(currentPassword, user.HashedPassword))
				return BadRequest(new {result = "Passwords do not match"});

			return true;

        }


        [HttpPut("edit-password/{newPassword}")]
		public IActionResult EditPassword(string newPassword)
		{
			string? token = Request.Headers["Authorization"];
			if (token == null || _userRepository.GetUserByToken(token) == null)
				return BadRequest(new { result = "no_token_sent" });

			User? existingUser = _userRepository.GetUserByToken(token);
			if (existingUser == null)
				return NotFound(new { result = "user_not_found" });

			existingUser.HashedPassword = _passwordHasher.HashPassword(newPassword);

			_userRepository.UpdateUser(existingUser);


			ProfileDTO profileDTO = new ProfileDTO
			{
				Id = existingUser.Id,
				FirstName = existingUser.FirstName,
				LastName = existingUser.LastName,
				BirthDate = existingUser.BirthDate,
				Sex = existingUser.Sex,
				PhoneNumber = existingUser.PhoneNumber,
				AboutMe = existingUser.AboutMe
			};

			return Ok(profileDTO);
		}

		[HttpPut("edit-profile-pic/{profileImageId}")]
		public IActionResult EditProfilePic(Guid profileImageId)
		{
			string? token = Request.Headers["Authorization"];
			if (token == null || _userRepository.GetUserByToken(token) == null)
				return BadRequest(new { result = "no_token_sent" });

			User? existingUser = _userRepository.GetUserByToken(token);
			if (existingUser == null)
				return NotFound(new { result = "user_not_found" });

			existingUser.Photo = _photoRepository.GetPhotoByPhotoId(profileImageId);
			existingUser.PhotoId = profileImageId;

			_userRepository.UpdateUser(existingUser);

			ProfileDTO profileDTO = new ProfileDTO
			{
				Id = existingUser.Id,
				FirstName = existingUser.FirstName,
				LastName = existingUser.LastName,
				BirthDate = existingUser.BirthDate,
				Sex = existingUser.Sex,
				PhoneNumber = existingUser.PhoneNumber,
				AboutMe = existingUser.AboutMe
			};

			return Ok(profileDTO);
		}

		[HttpGet("search-users/{name}")]
		public ActionResult<IEnumerable<User>> GetAllUserBySearch(string name)
		{
			List<User> matches = _userRepository.GetUserListBySearchName(name);

			if (matches == null)
			{
				return BadRequest(new { result = "no_matching_users_found" });
			}

			return Ok(matches);
		}


		/*[HttpPut("edit-about-me/{userId}")]
		public async Task<IActionResult> EditAboutMe(int userId, [FromBody] AboutMeDTO aboutMeDTO)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new { result = "invalid_user" });
			}

			User? existingUser = await _userRepository.GetUserById(userId);

			if (existingUser == null)
			{
				return NotFound(new { result = "user_not_found" });
			}

			existingUser.AboutMe = aboutMeDTO.AboutMe;


			_userRepository.UpdateUser(existingUser);


			return Ok(existingUser);
		}*/


	}
}
