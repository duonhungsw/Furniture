using System.Security.Claims;

namespace Furniture.API.Controllers;

[Route("orders")]
public class OrderController(
	IOrderService _service,
	ITokenService _tokenService) : BaseApiController
{
	[HttpPost("create")]
	public async Task<bool> CreateOrder([FromBody] CreateOrderDto model)
	{
		var result = await _service.CreateOrderAsync(model);
		return result;
	}
	[HttpGet("{accountId}/purchase")]
	public async Task<List<OrderDto>> GetPurchaseOfAccounts([FromRoute] Guid accountId, [FromQuery] Guid statusId, [FromQuery] QueryInfo queryInfo)
	{
		var results = await _service.GetOrdersAsync(accountId, queryInfo, statusId);
		return results;
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
