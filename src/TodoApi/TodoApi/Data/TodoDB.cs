using Microsoft.EntityFrameworkCore;
using TodoApi.Entities;

namespace TodoApi.Data
{
	class TodoDB : DbContext
	{
		public TodoDB(DbContextOptions<TodoDB> options) : base(options) { }

		public DbSet<Todo> TodoList => Set<Todo>();

	}
}
