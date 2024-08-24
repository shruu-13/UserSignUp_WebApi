using System.Threading.Tasks;
namespace UserSignUp_WebApi.Services


{
	public interface IEmailService
	{
		Task SendVerificationEmailAsync(string email, string verificationToken);

		public interface IEmailService
		{
			Task SendVerificationEmailAsync(string email, string token);
		}
	}
}
