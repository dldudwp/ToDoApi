using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TodoApi.Data;
using TodoApi.Entities;
using TodoApi.Services;
using TodoApi.Services.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDB>(options =>
{
	options.UseInMemoryDatabase("Todo");
});

builder.Services.AddSingleton<ITokenService>(new TokenService());
builder.Services.AddSingleton<IUserRepositoryService>(new UserRepositoryService());

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
	opt.TokenValidationParameters = new TokenValidationParameters()
	{
		ValidateIssuerSigningKey = true,
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidIssuer = builder.Configuration["Jwt:Issuer"],
		ValidAudience = builder.Configuration["Jwt:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
	};
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	var securityScheme = new OpenApiSecurityScheme
	{
		Name = "JWT 인증",
		Description = "JWT Bearer token을 입력하세요",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.Http,
		Scheme = "bearer",
		BearerFormat = "JWT",
		Reference = new OpenApiReference
		{
			Id = JwtBearerDefaults.AuthenticationScheme,
			Type = ReferenceType.SecurityScheme
		}
	};

	c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{ 
			securityScheme, new string[] { } 
		}
	});
});

builder.Services.AddMvc();

var app = builder.Build();

app.MapGet("/", () => "Hello Demo");

app.MapGet("/todo", async (TodoDB db) => await db.TodoList.ToListAsync())
	.Produces<List<Todo>>(StatusCodes.Status200OK)
	.WithName("GetAllTodoList").WithTags("Getters");

app.MapPost("/todo", async ([FromBody] Todo todo, [FromServices] TodoDB db, HttpResponse response) =>
{
	db.TodoList.Add(todo);
	await db.SaveChangesAsync();

	return Results.Created($"todo/{todo.Id}", todo);
}).Accepts<Todo>("application/json")
  .Produces<Todo>(StatusCodes.Status201Created);

app.MapPut("/todo", async (int todoId, string title, [FromServices] TodoDB db, HttpResponse response) =>
{
	var todo = db.TodoList.SingleOrDefault(s => s.Id == todoId);

	if (todo == null) return Results.NotFound();

	todo.Title = title;
	await db.SaveChangesAsync();
	return Results.Created("/todo", todo);
});

app.MapGet("/todo/{id}", async (TodoDB db, int id) =>
	await db.TodoList.SingleOrDefaultAsync(s => s.Id == id) is Todo todo ? Results.Ok(todo) : Results.NotFound());

app.MapGet("/todo/search/{query}", (string query, TodoDB db) =>
{
	var todolist = db.TodoList.Where(x => x.Title.ToLower().Contains(query.ToLower())).ToList();
	return todolist.Count > 0 ? Results.Ok(todolist) : Results.NotFound();
});

//Login
app.MapPost("/login", [AllowAnonymous] async ([FromBody] UserModel userModel, ITokenService tokenservice, IUserRepositoryService userRepositoryService, HttpResponse response) =>
{
	var userDto = userRepositoryService.GetUser(userModel);

	if (userDto == null)
	{
		response.StatusCode = 401;
		return;
	}

	var issuer = builder.Configuration["Jwt:Issuer"];
	var audience = builder.Configuration["Jwt:Audience"];
	var key = builder.Configuration["Jwt:Key"];

	var token = tokenservice.BuildToken(key, issuer, audience, userDto);

	await response.WriteAsJsonAsync(new { token = token });

	return;

}).Produces(StatusCodes.Status200OK).WithName("Login").WithTags("Accounts");

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

ap

app.UseSwagger();
app.UseSwaggerUI();

app.Run();

