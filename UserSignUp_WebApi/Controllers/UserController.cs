using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UserSignUp_WebApi.Models;
using UserSignUp_WebApi.Data;
using UserSignUp_WebApi.Services;

namespace UserSignUp_WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly DataContext _context;
		private readonly EmailService _emailService;

		public UserController(DataContext context, EmailService emailService)
		{
			_context = context;
			_emailService = emailService;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register(UserRegisterRequest request)
		{
			if (_context.Users.Any(u => u.Email == request.Email))
			{
				return BadRequest("User already exists.");
			}

			CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

			var user = new User
			{
				Email = request.Email,
				PasswordHash = passwordHash,
				PassworfSalt = passwordSalt,
				VerificationToken = CreateRandomToken()
			};

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			// Send verification email
			var verificationLink = Url.Action("Verify", "User", new { token = user.VerificationToken }, Request.Scheme);
			await _emailService.SendEmailAsync(user.Email, "Verify your email", $"Please verify your email by clicking on this link: {verificationLink}");

			return Ok("User successfully created! Please check your email to verify.");
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(UserLoginRequest request)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
			if (user == null)
			{
				return BadRequest("User not found.");
			}

			if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PassworfSalt))
			{
				return BadRequest("Password is incorrect.");
			}

			if (user.VerifiedAt == null)
			{
				return BadRequest("Email not verified.");
			}

			return Ok($"Welcome back, {user.Email}! :)");
		}

		[HttpPost("verify")]
		public async Task<IActionResult> Verify(string token)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
			if (user == null)
			{
				return BadRequest("Invalid token.");
			}

			user.VerifiedAt = DateTime.Now;
			user.VerificationToken = null; // Clear the token after verification
			await _context.SaveChangesAsync();

			return Ok("Email verified successfully!");
		}

		[HttpPost("forgot-password")]
		public async Task<IActionResult> ForgotPassword(string email)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
			if (user == null)
			{
				return BadRequest("User not found.");
			}

			user.PasswordResetToken = CreateRandomToken();
			user.ResetTokenExpires = DateTime.Now.AddHours(1); // Token valid for 1 hour
			await _context.SaveChangesAsync();

			// Send reset password email
			var resetLink = Url.Action("ResetPassword", "User", new { token = user.PasswordResetToken }, Request.Scheme);
			await _emailService.SendEmailAsync(user.Email, "Reset your password", $"Please reset your password by clicking on this link: {resetLink}");

			return Ok("Reset password link sent to your email.");
		}

		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == request.Token && u.ResetTokenExpires > DateTime.Now);
			if (user == null)
			{
				return BadRequest("Invalid or expired token.");
			}

			CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

			user.PasswordHash = passwordHash;
			user.PassworfSalt = passwordSalt;
			user.PasswordResetToken = null;
			user.ResetTokenExpires = null;
			await _context.SaveChangesAsync();

			return Ok("Password successfully reset.");
		}

		private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
		{
			using (var hmac = new HMACSHA256())
			{
				passwordSalt = hmac.Key;
				passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
			}
		}

		private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
		{
			using (var hmac = new HMACSHA256(passwordSalt))
			{
				var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
				return computedHash.SequenceEqual(passwordHash);
			}
		}

		private string CreateRandomToken()
		{
			return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
		}
	}
}
