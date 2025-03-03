namespace Furniture.Common;

public class BadRequestException : CustomException
{
	public BadRequestException(string message = "Bad Request")
		: base(message, StatusCodes.Status400BadRequest) { }
}
