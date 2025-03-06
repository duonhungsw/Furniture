using Microsoft.AspNetCore.Authorization;

namespace Furniture.API.Controllers;

[Route("orders")]
public class OrderController(
	IOrderService _service,
	ITokenService _tokenService) : BaseApiController
{
	[Authorize]
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
	[HttpPatch("{orderId}/change-status")]
	public async Task<bool> ChangeStatus([FromRoute] Guid orderId)
	{
		var account = await _tokenService.Authenticate();
		var result = await _service.ChangeStatusAsync(orderId, account.RoleName);
		return result;
	}
	[HttpGet("checkout")]
	public async Task<List<OrderCheckout>> GetOrdersForAccount(Guid id)
	{
		var results = await _service.GetOrdersForAccountAsync(id);
		return results;
	}
}
