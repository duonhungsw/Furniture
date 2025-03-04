using Google.Apis.Auth;

namespace Furniture.Service;

public class AuthService(
	IConfiguration _configuration,
	IAccountRepository _accountRepository,
	ITokenService _tokenService) : IAuthService
{
	public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleTokenAsync(string idToken)
	{
		var settings = new GoogleJsonWebSignature.ValidationSettings
		{
			Audience = new[] { _configuration["Authentication:Google:ClientId"] }
		};

		return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
	}

	public async Task<Account> GetOrCreateAccountAsync(GoogleJsonWebSignature.Payload payload)
	{
		var account = await _accountRepository.GetByEmailAsync(payload.Email);
		if (account == null)
		{
			account = new Account
			{
				Id = Guid.NewGuid(),
				Email = payload.Email,
				Name = payload.Name,
				HashPassword = PasswordHasher.HashPasswordPBKDF2(payload.Email),
				RoleName = AppRoles.Customer.ToString()
			};
			try
			{
				_accountRepository.Create(account);
				await _accountRepository.SaveChangesAsync();
			}
			catch
			{
				throw new InternalServerException();
			}
		}
		return account;
	}
	public string GenerateJwtTokenAsync(Account account)
	=>  _tokenService.GenerateAccessToken(account);

	public string GenerateRefreshTokenAsync()
	 => _tokenService.GenerateRefreshToken();
}
