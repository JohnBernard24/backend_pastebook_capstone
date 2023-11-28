using backend_pastebook_capstone.AuthenticationService.Models;
using backend_pastebook_capstone.AuthenticationService.Repository;
using backend_pastebook_capstone.AuthenticationService.TokenGenerators;
using backend_pastebook_capstone.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace backend_pastebook_capstone.AuthenticationService.Authenticator
{
	public class Authenticator
	{
		private readonly AccessTokenGenerator _accessTokenGenerator;
		private readonly AccessTokenRepository _accessTokenRepository;
		private readonly AuthenticationConfiguration _configuration;

		public Authenticator(AccessTokenGenerator accessTokenGenerator, AccessTokenRepository accessTokenRepository, AuthenticationConfiguration configuration)
		{
			_accessTokenGenerator = accessTokenGenerator;
			_accessTokenRepository = accessTokenRepository;
			_configuration = configuration;
		}


		public string Authenticate(User user)
		{
			string token = _accessTokenGenerator.GenerateToken(user);

			AccessToken accessToken = new AccessToken
			{
				Token = token,
				UserId = user.Id
			};

			_accessTokenRepository.InsertToken(accessToken);

			return token;
		}

		public bool Validate(string token)
		{
			TokenValidationParameters validatorParameters = new TokenValidationParameters()
			{
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.AccessTokenSecret)),
				ValidIssuer = _configuration.Issuer,
				ValidAudience = _configuration.Audience,
				ValidateIssuerSigningKey = true,
				ValidateIssuer = true,
				ValidateAudience = true,
				ClockSkew = TimeSpan.Zero
			};

			JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

			try
			{
				tokenHandler.ValidateToken(token, validatorParameters, out SecurityToken validatedToken);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
