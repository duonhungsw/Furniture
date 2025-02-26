
﻿using Microsoft.AspNetCore.Mvc;

namespace Furniture.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BaseApiController : ControllerBase
{
	protected ActionResult<PagedResult<T>> CreatePagedResult<T>(IEnumerable<T> list, int pageIndex, int pageSize)
	{
		int totalCount = list.Count();
		var items = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

		var result = new PagedResult<T>
		{
			Items = items,
			PageIndex = pageIndex,
			PageSize = pageSize,
			TotalCount = totalCount,
			TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
		};

		return Ok(result);
	}
}
public class PagedResult<T>
{
	public List<T> Items { get; set; } = new();
	public int PageIndex { get; set; }
	public int PageSize { get; set; }
	public int TotalCount { get; set; }
	public int TotalPages { get; set; }
}
