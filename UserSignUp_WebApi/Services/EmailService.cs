using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Threading.Tasks;

namespace UserSignUp_WebApi.Services
{
	public class EmailService
	{
		private readonly IConfiguration _configuration;

		public EmailService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task SendEmailAsync(string toEmail, string subject, string body)
		{
			var emailSettings = _configuration.GetSection("EmailSettings");
			var smtpServer = emailSettings["SmtpServer"];
			var smtpPort = int.Parse(emailSettings["SmtpPort"]);
			var smtpUser = emailSettings["SmtpUser"];
			var smtpPass = emailSettings["SmtpPass"];

			var message = new MimeMessage();
			message.From.Add(new MailboxAddress("Your App", smtpUser));
			message.To.Add(new MailboxAddress("", toEmail));
			message.Subject = subject;
			message.Body = new TextPart("plain") { Text = body };

			using (var client = new SmtpClient())
			{
				await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
				await client.AuthenticateAsync(smtpUser, smtpPass);
				await client.SendAsync(message);
				await client.DisconnectAsync(true);
			}
		}
	}
}
