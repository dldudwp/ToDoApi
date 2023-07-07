using TodoApi.Entities;

namespace TodoApi.Services.Contracts
{
	public interface IUserRepositoryService
	{
		User? GetUser(UserModel model);
	}
}
