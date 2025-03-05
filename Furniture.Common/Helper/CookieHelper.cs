namespace Furniture.Common;

public class CookieHelper
{
	public static CookieOptions GetCustomCookieOptions(int days, bool secure)
	{
		return new CookieOptions
		{
			HttpOnly = true,
			Secure = secure,
			SameSite = SameSiteMode.None,
			Expires = DateTime.UtcNow.AddDays(days)
		};
	}
}
