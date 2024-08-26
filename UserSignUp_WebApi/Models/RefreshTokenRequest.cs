using System.ComponentModel.DataAnnotations;

namespace UserSignUp_WebApi.Models
{
	public class RefreshTokenRequest
	{
		[Required]
		public string RefreshToken { get; set; } = string.Empty;
	}
}
