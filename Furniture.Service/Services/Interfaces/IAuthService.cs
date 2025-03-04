using Google.Apis.Auth;

namespace Furniture.Service;

public interface IAuthService
{
	Task<GoogleJsonWebSignature.Payload> VerifyGoogleTokenAsync(string idToken);
	Task<Account> GetOrCreateAccountAsync(GoogleJsonWebSignature.Payload payload);
	string GenerateJwtTokenAsync(Account account);
	string GenerateRefreshTokenAsync();
}
