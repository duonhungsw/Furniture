using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;
using System.Security.Claims;

namespace Furniture.API.Extensions;

public static class ClaimsPrincipleExtensions
{

	public static async Task<Account> GetUserByEmail(this UserManager<Account> manager, ClaimsPrincipal user)
	{
		var EmailToReturn = await manager.Users.FirstOrDefaultAsync(x => x.Email == user.GetEmail());

		if (EmailToReturn == null) throw new AuthenticationException("User not found");
		return EmailToReturn;
	}

	public static string GetEmail(this ClaimsPrincipal user)
	{
		var email = user.FindFirstValue(ClaimTypes.Email) ?? throw new AuthenticationException("Email claim not found");
		return email;
	}
	public static AccountDto GetUser(this ClaimsPrincipal user)
	{
		if (user == null) throw new ArgumentNullException(nameof(user));

		var result = new AccountDto
		{
			Id = Guid.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : Guid.Empty,
			Name = user.FindFirst(ClaimTypes.Name)?.Value,
			Email = user.FindFirst(ClaimTypes.Email)?.Value,
			RoleName = user.FindFirst(ClaimTypes.Role)?.Value,
		};

		return result;
	}

}
