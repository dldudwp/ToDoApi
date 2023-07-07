using TodoApi.Entities;
using TodoApi.Services.Contracts;

namespace TodoApi.Services
{
	public class UserRepositoryService : IUserRepositoryService
	{
		public List<User> _users = new List<User>()
	{
		new("admin","1234")
	};

		public User? GetUser(UserModel model) =>
			_users.FirstOrDefault(x =>
			string.Equals(x.UserName, model.UserName) && string.Equals(x.Password, model.Password)
			);
	}
}
