using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TodoApi.Entities;
using TodoApi.Services.Contracts;

namespace TodoApi.Services;

public class TokenService : ITokenService
{
	private TimeSpan ExpriryDuration = new TimeSpan(0, 10, 0);

	public string BuildToken(string key, string issuer, string audience, User user)
	{
		var claims = new[]
		{
			new Claim(ClaimTypes.Name, user.UserName),
			new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
		};

		var mySecret = Encoding.UTF8.GetBytes(key);
		var securityKey = new SymmetricSecurityKey(mySecret);
		var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
		var tokenDescriptor = new JwtSecurityToken(issuer, audience, claims,
			expires: DateTime.Now.Add(ExpriryDuration), signingCredentials: credentials);

		return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
	}
}
