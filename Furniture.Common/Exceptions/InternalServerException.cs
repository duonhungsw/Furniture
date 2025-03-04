namespace Furniture.Common;

public class InternalServerException : CustomException
{
	public InternalServerException(string message = "Internal server error") 
		: base(message, StatusCodes.Status500InternalServerError) { }
}
