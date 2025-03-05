using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Furniture.Service;

public class JwtTokenService : ITokenService
{
	private readonly string AccessToken = "AccessToken";
	private readonly string RefreshToken = "RefreshToken";
	private readonly string _secretKey;
	private readonly string _issuer;
	private readonly string _audience;
	private readonly IHttpContextAccessor _httpContextAccessor;

	public JwtTokenService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
		_secretKey = configuration["JwtSettings:SecretKey"]!;
		_issuer = configuration["JwtSettings:ValidIssuer"]!;
		_audience = configuration["JwtSettings:ValidAudience"]!;
	}
	public async Task<Account?> GetTokenAsync()
	{
		try
		{
			var context = _httpContextAccessor.HttpContext;
			if (context == null)
			{
				return null;
			}

			var accessToken = context.Request.Cookies[AccessToken];
			var refreshToken = context.Request.Cookies[RefreshToken];

			if (string.IsNullOrEmpty(accessToken))
			{
				return null;
			}

			var principalAccessToken = ValidateToken(accessToken);
			if (principalAccessToken == null || IsTokenExpired(accessToken))
			{
				if (string.IsNullOrEmpty(refreshToken) || IsTokenExpired(refreshToken))
				{
					RemoveTokenInCookie();
					return null;
				}

				var emailAccount = GetClaimValue(principalAccessToken!, ClaimTypes.Email);
				var idClaim = GetClaimValue(principalAccessToken!, ClaimTypes.NameIdentifier);
				var nameClaim = GetClaimValue(principalAccessToken!, ClaimTypes.Name);
				var roleClaim = GetClaimValue(principalAccessToken!, ClaimTypes.Role);

				if (string.IsNullOrEmpty(emailAccount) || string.IsNullOrEmpty(idClaim) ||
					string.IsNullOrEmpty(nameClaim) || string.IsNullOrEmpty(roleClaim))
				{
					RemoveTokenInCookie();
					return null;
				}

				var customerDto = new Account
				{
					Id = new Guid(idClaim),
					Name = nameClaim,
					Email = emailAccount,
					RoleName = roleClaim
				};

				var newAccessToken = GenerateAccessToken(customerDto);
				SetTokensInsideCookie(newAccessToken, refreshToken);

				return customerDto;
			}

			return new Account
			{
				Id = new Guid(GetClaimValue(principalAccessToken, ClaimTypes.NameIdentifier)!),
				Name = GetClaimValue(principalAccessToken, ClaimTypes.Name)!,
				Email = GetClaimValue(principalAccessToken, ClaimTypes.Email)!,
				RoleName = GetClaimValue(principalAccessToken, ClaimTypes.Role)!
			};
		}
		catch (SecurityTokenException)
		{
			RemoveTokenInCookie();
			return null;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error in GetTokenAsync: {ex.Message}");
			return null;
		}
	}


	private string? GetClaimValue(ClaimsPrincipal principal, string claimType)
	{
		return principal.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
	}


	public string GenerateAccessToken(Account customerDto)
	{
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.NameIdentifier, customerDto!.Id.ToString()),
			new Claim(JwtRegisteredClaimNames.Iat,
						new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
						ClaimValueTypes.Integer64),
			new Claim(ClaimTypes.Name, customerDto.Name!),
			new Claim(ClaimTypes.Email, customerDto.Email!),
			new Claim(ClaimTypes.Role, customerDto.RoleName!),
		};

		var token = new JwtSecurityToken(
			issuer: _issuer,
			audience: _audience,
			claims: claims,
			expires: DateTime.UtcNow.AddDays(1),
			signingCredentials: creds)
		;

		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	public string GenerateRefreshToken()
	{
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var token = new JwtSecurityToken(
			issuer: _issuer,
			audience: _audience,
			expires: DateTime.UtcNow.AddDays(7),
			signingCredentials: creds);


		return new JwtSecurityTokenHandler().WriteToken(token);
	}
	public void SetTokensInsideCookie(string accessToken, string refreshToken)
	{
		_httpContextAccessor.HttpContext?.Response.Cookies.Append(AccessToken, accessToken, CookieHelper.GetCustomCookieOptions(1, true));
		_httpContextAccessor.HttpContext?.Response.Cookies.Append(RefreshToken, refreshToken, CookieHelper.GetCustomCookieOptions(7, true));
	}
	public void RemoveTokenInCookie()
	{
		_httpContextAccessor.HttpContext?.Response.Cookies.Delete(AccessToken);
		_httpContextAccessor.HttpContext?.Response.Cookies.Delete(RefreshToken);
	}
	public async Task<Account> Authenticate()
	{
		var customer = await GetTokenAsync();
		return customer!;
	}
	private ClaimsPrincipal ValidateToken(string token)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.UTF8.GetBytes(_secretKey);

		try
		{
			var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = _issuer,
				ValidAudience = _audience,
				IssuerSigningKey = new SymmetricSecurityKey(key)
			}, out _);

			return principal;
		}
		catch
		{
			throw new UnauthorizedAccessException("Token is validation");
		}
	}
	private bool IsTokenExpired(string token)
	{
		try
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.UTF8.GetBytes(_secretKey);

			tokenHandler.ValidateToken(token, new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(key),
				ValidateIssuer = false,
				ValidateAudience = false,
				ClockSkew = TimeSpan.Zero
			}, out var validatedToken);

			return false;
		}
		catch (SecurityTokenExpiredException)
		{
			return true;
		}
		catch
		{
			throw new Exception("Invalid token.");
		}
	}

	public string GeneratePasswordResetTokenAsync(AccountDto accountDto)
	{
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.NameIdentifier, accountDto!.Id.ToString()),
			new Claim(JwtRegisteredClaimNames.Iat,
						new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
						ClaimValueTypes.Integer64),
			new Claim(ClaimTypes.Name, accountDto.Name!),
			new Claim(ClaimTypes.Email, accountDto.Email!),
			new Claim(ClaimTypes.Role, accountDto.RoleName!),
		};

		var token = new JwtSecurityToken(
			issuer: _issuer,
			audience: _audience,
			claims: claims,
			expires: DateTime.UtcNow.AddMinutes(10),
			signingCredentials: creds);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}
