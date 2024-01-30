using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend_pastebook_capstone.AuthenticationService.TokenGenerators
{
	public class TokenGenerator
	{
		public string GenerateToken(string secretKey, string issuer, string audience, int expirationMinutes, IEnumerable<Claim>? claims = null)
		{
			SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

			SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


			JwtSecurityToken token = new JwtSecurityToken(
				issuer,
				audience,
				claims,
				DateTime.UtcNow,
				DateTime.UtcNow.AddMinutes(expirationMinutes),
				credentials
				);
			// Generate our token
			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
