namespace Furniture.API.Helper;

public class Pagination<T>(int count, int pageIndex, int pageSize, IReadOnlyList<T> items)
{
	public int count { get; set; } = count;
	public int PageIndex { get; set; } = pageIndex;
	public int PageSize { get; set; } = pageSize;
	public IReadOnlyList<T> Items { get; set; } = items;
}
