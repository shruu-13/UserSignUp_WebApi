using System.Threading.Tasks;

namespace UserSignUp_WebApi.Services
{
	public interface IEmailService
	{
		Task SendEmailAsync(string toEmail, string subject, string body);
	}
}
