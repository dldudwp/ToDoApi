using System.ComponentModel.DataAnnotations;

namespace TodoApi.Entities
{
	public record User(string UserName, string Password);

	public record UserModel
	{
		public UserModel(string userName, string password)
		{
			UserName = userName;
			Password = password;
		}

		[Required]
		public string UserName { get; set; }
		[Required]
		public string Password { get; set; }
	}

}
