using TodoApi.Entities;

namespace TodoApi.Services.Contracts
{
	public interface ITokenService
	{
		string BuildToken(string key, string issuer, string audience, User userDto);
	}
}
