using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace UserSignUp_WebApi.Services
{
	public interface ITokenService
	{
		string GenerateAccessToken(string email);
		string GenerateRefreshToken();
		ClaimsPrincipal? ValidateRefreshToken(string refreshToken);
		void InvalidateRefreshToken(string refreshToken); // Optional
	}
}
