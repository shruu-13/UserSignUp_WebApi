global using Microsoft.EntityFrameworkCore;
global using UserSignUp_WebApi.Models;
global using UserSignUp_WebApi.Data;
global using UserSignUp_WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register EmailService with configuration from appsettings.json
var emailSettings = builder.Configuration.GetSection("EmailSettings");
builder.Services.AddSingleton<EmailService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
