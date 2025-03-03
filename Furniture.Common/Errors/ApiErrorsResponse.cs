namespace Furniture.Common;

public class ApiErrorsResponse(int status, string message, string details)
{
    public  int status { get; set; } = status;
    public  string message { get; set; } = message;
    public  string details { get; set; } = details;
}
