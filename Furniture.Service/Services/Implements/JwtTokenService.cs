using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Furniture.Service.Services.Implements;

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
		_secretKey = configuration["JwtSettings:SecretKey"]!;
		_issuer = configuration["JwtSettings:ValidIssuer"]!;
		_audience = configuration["JwtSettings:ValidAudience"]!;
		_httpContextAccessor = httpContextAccessor;
	}
	public async Task<Account?> GetTokenAsync()
	{
		try
		{
			var accessToken = _httpContextAccessor.HttpContext?.Request.Cookies[AccessToken];
			if (accessToken == null)
			{
				return null;
			}
			var principalAccessToken = ValidateToken(accessToken!);
			if (IsTokenExpired(accessToken))
			{
				var emailAccount = principalAccessToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

				var refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies[RefreshToken];

				if (string.IsNullOrEmpty(refreshToken!.ToString()) || IsTokenExpired(refreshToken.ToString()!))
				{
					RemoveTokenInCookie();
					return null;
				}

				if (principalAccessToken == null)
				{
					RemoveTokenInCookie();
					return null;
				}
				var customerDto = new Account
				{
					Id = new Guid(principalAccessToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!),
					Name = principalAccessToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value!,
					Email = principalAccessToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value!,
					RoleName = principalAccessToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value!,
				};

				var newAccessToken = GenerateAccessToken(customerDto);
				SetTokensInsideCookie(newAccessToken, refreshToken);

				return await Task.FromResult(customerDto) ;
			}

			var result = new Account
			{
				Id = new Guid(principalAccessToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!),
				Name = principalAccessToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value!,
				Email = principalAccessToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value!,
				RoleName = principalAccessToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value!,
			};
			return result;
		}
		catch
		{
			RemoveTokenInCookie();
			return null;
		}
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
			signingCredentials: creds);

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
		var cookieOptions = new CookieOptions
		{
			HttpOnly = true,
			Secure = true,
			SameSite = SameSiteMode.Strict,
		};
		_httpContextAccessor.HttpContext?.Response.Cookies.Append(AccessToken, accessToken, cookieOptions);
		_httpContextAccessor.HttpContext?.Response.Cookies.Append(RefreshToken, refreshToken, cookieOptions);
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
	
}
