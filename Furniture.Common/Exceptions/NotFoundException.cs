namespace Furniture.Common;

public class NotFoundException : CustomException
{
	public NotFoundException(string message = "Resource not found")
	   : base(message, StatusCodes.Status404NotFound) { }
}
