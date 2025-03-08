namespace Furniture.Common;

public class ValidationErrorResponse
{
	public string Title { get; set; } = string.Empty;
	public int Status { get; set; }
	public Dictionary<string, List<string>>? Errors { get; set; }
}
