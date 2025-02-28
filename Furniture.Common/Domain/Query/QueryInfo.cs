namespace Furniture.Common;

public class QueryInfo
{
	public int PageIndex { get; set; } = AppConstants.DefaultPageIndex;
	public int PageSize { get; set; } = AppConstants.DefaultPageSize;
	public string? SearchText { get; set; }
}
