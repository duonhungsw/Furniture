using Furniture.Common.Domain.Query;
using Microsoft.AspNetCore.Mvc;

namespace Furniture.API.Controllers;

[Route("/[controller]")]
[ApiController]
public class BaseApiController : ControllerBase
{
	protected ActionResult<PagedResult<T>> CreatePagedResult<T>(IEnumerable<T> list, QueryInfo queryInfo)
	{
		int totalCount = list.Count();
		var items = list.Skip((queryInfo.PageIndex - 1) * queryInfo.PageSize).Take(queryInfo.PageSize).ToList();

		var result = new PagedResult<T>
		{
			Items = items,
			PageIndex = queryInfo.PageIndex,
			PageSize = queryInfo.PageSize,
			TotalCount = totalCount,
			TotalPages = (int)Math.Ceiling(totalCount / (double)queryInfo.PageSize)
		};

		return Ok(result);
	}
}
//public class PagedResult<T>
//{
//	public List<T> Items { get; set; } = new();
//	public int PageIndex { get; set; }
//	public int PageSize { get; set; }
//	public int TotalCount { get; set; }
//	public int TotalPages { get; set; }
//}
