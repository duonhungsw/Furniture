namespace Furniture.Common;

public class UnauthorizedException : CustomException
{
	public UnauthorizedException(string message = "Unauthorized")
		: base(message, StatusCodes.Status401Unauthorized) { }
}
