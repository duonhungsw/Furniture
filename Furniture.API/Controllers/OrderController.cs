using Furniture.Core.Dtos.Order;
using Microsoft.AspNetCore.Mvc;

namespace Furniture.API.Controllers;

public class OrderController(IOrderService _service) : BaseApiController
{
	[HttpPost("create")]
	public async Task<bool> CreateOrder([FromBody] CreateOrderDto model)
	{
		var result = await _service.CreateOrderAsync(model);
		return result;
	}
	[HttpGet("paging")]
	public async Task<ActionResult<PagedResult<OrderItemDto>>> GetOrdersWithPaging([FromQuery] QueryInfo queryInfo)
	{
		var results = await _service.GetOrdersAsync();
		return CreatePagedResult(results, queryInfo);
	}
}
