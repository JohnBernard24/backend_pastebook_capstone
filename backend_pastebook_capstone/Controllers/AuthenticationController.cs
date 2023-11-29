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

		public AuthenticationController(UserRepository userRepository, BcryptPasswordHasher passwordHasher, Authenticator authenticator, TimelineRepository timelineRepository, AlbumRepository albumRepository, AccessTokenRepository accessTokenRepository)
		{
			_passwordHasher = passwordHasher;
			_authenticator = authenticator;

			_userRepository = userRepository;
			_timelineRepository = timelineRepository;
			_albumRepository = albumRepository;
			_accessTokenRepository = accessTokenRepository;
		}


		[HttpPost("login")]
		public IActionResult Login([FromBody] UserLoginDTO userLoginDTO)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new { result = "invalid_user_login" });
			}

			User? user = _userRepository.GetUserByEmail(userLoginDTO.Email);
			if(user == null)
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
			if(existingUser != null)
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
			if(token == null)
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
			if(token == null || _userRepository.GetUserByToken(token) == null)
			{
				return BadRequest(new { result = "no_valid_token_sent" });
			}
			return Ok(_authenticator.Validate(token));
		}

		[HttpPost("verify-email/{recipientEmail}")]
		public async Task<IActionResult> SendEmail(string recipientEmail)
		{
			var senderEmail = "teametivacpastebook@gmail.com";
			var senderPassword = "nbci cmzt wqds krbv";

			User? user = _userRepository.GetUserByEmail(recipientEmail);
			if (user == null)
				return BadRequest(new { result = "no_account_with_that_email" });

			var message = new MailMessage(senderEmail, recipientEmail)
			{
				Subject = $"Verify your email, {user.Email}!",
				IsBodyHtml = true,
				Body =
				$@"
					<html>
					<head>
					<title>Pastebook Email Confirmation</title>
					<style>
					body {{
					  font-family: sans-serif;
					  margin: 0;
					  padding: 0;
					}}

					.container {{
					  width: 600px;
					  margin: 0 auto;
					}}

					h1 {{
					  text-align: center;
					  font-size: 30px;
					  margin-top: 40px;
					}}

					p {{
					  font-size: 16px;
					  line-height: 1.5;
					}}

					a {{
					  color: #fff;
					  background-color: #f9a113;
					  padding: 10px 20px;
					  border-radius: 4px;
					  text-decoration: none;
					}}

					.footer {{
					  text-align: center;
					  font-size: 12px;
					  margin-top: 40px;
					}}
					</style>
					</head>
					<body>
					  <div class=""container"">
						<img style='width: 100;' src = 'https://cdn.discordapp.com/attachments/1174240282951303239/1176866765209337886/Logo1_dark.PNG?ex=65706d95&is=655df895&hm=62cdfca8d61e6fd565803260716f0493e37f4d764e82c5b5d8e11f9e861783e3&' alt = 'Pastebook Logo'>
						<h1>Email Confirmation</h1>
						<p>
						  Hey {user.FirstName + " " + user.LastName}, you're almost ready!
						</p>
						<p>
						  Simply click the big button below to verify your email address.
						</p>
						<a href=""http://localhost:4200/forgot-password/{user.Id}"" style=""display:inline-block;padding:10px 20px;background-color:#007BFF;color:#ffffff;text-decoration:none;border-radius:5px;"">Confirm Email</a>
						<div class=""footer"">
						  Copyright © 2023 Team Etivac. All rights reserved. Email sent by Pastebook.com.
						</div>
					  </div>
					</body>
					</html>
				"
			};

			var smtpClient = new SmtpClient("smtp.gmail.com")
			{
				Port = 587,
				Credentials = new NetworkCredential(senderEmail, senderPassword),
				EnableSsl = true,
			};

			try
			{
				await smtpClient.SendMailAsync(message);
				return Ok(new { result = "Email sent successfully!" });
			}
			catch (Exception)
			{
				return BadRequest(new { result = "Error sending email." });
			}
		}


	}
}
