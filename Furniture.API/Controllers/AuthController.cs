using Microsoft.AspNetCore.Mvc;

namespace Furniture.API.Controllers;

[Route("signin")]
public class AuthController(IAuthService _service) : BaseApiController
{
	[HttpGet("google")]
	public async Task<TokenResponse> SignInGoogle([FromQuery] string idToken)
	{
		try
		{
			var payload = await _service.VerifyGoogleTokenAsync(idToken);
			var account = await _service.GetOrCreateAccountAsync(payload);
			var jwtToken =  _service.GenerateJwtTokenAsync(account);
			var refreshToken = _service.GenerateRefreshTokenAsync();
			return new TokenResponse
			{
				AccessToken = jwtToken,
				RefreshToken = refreshToken,
			};
		}
		catch (Exception ex)
		{
			throw new InternalServerException(ex.Message);
		}
	}
}
