namespace Furniture.Common;

public class FilterProductInfo
{
    public List<string>? Brands { get; set; }
    public List<string>? Types { get; set; }
	public int PageIndex { get; set; } = AppConstants.DefaultPageIndex;
	public int PageSize { get; set; } = AppConstants.DefaultPageSize;
	public string? SearchText { get; set; }
	public string? OrderBy {  get; set; } = AppConstants.DefaultProductOrderBy;
}
