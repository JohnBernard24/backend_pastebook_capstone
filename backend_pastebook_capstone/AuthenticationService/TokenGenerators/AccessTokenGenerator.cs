using backend_pastebook_capstone.AuthenticationService.Models;
using backend_pastebook_capstone.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace backend_pastebook_capstone.AuthenticationService.TokenGenerators
{
	public class AccessTokenGenerator
	{
		private readonly AuthenticationConfiguration _configuration;
		private readonly TokenGenerator _tokenGenerator;


		public AccessTokenGenerator(AuthenticationConfiguration configuration, TokenGenerator tokenGenerator)
		{
			_configuration = configuration;
			_tokenGenerator = tokenGenerator;
		}

		public string GenerateToken(User user)
		{
			SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.AccessTokenSecret));
			SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


			List<Claim> claims = new List<Claim>()
			{
				new Claim("id", user.Id.ToString()),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName)
			};

			return _tokenGenerator.GenerateToken(
				_configuration.AccessTokenSecret,
				_configuration.Issuer,
				_configuration.Audience,
				_configuration.AccessTokenExpirationMinutes,
				claims
				);
		}
	}
}
