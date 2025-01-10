using System.Security.Claims;

namespace Furniture.Service.Services.Interfaces;

public interface ITokenService
{
	string GenerateAccessToken(Account accountDto);
	string GenerateRefreshToken();
	//ClaimsPrincipal ValidateToken(string token);
	void SetTokensInsideCookie(string accessToken, string refreshToken);
	public Task<Account?> GetTokenAsync();
	void RemoveTokenInCookie();
	Task<Account> Authenticate();
	//bool IsTokenExpired(string token);
}
