using Furniture.Core.Dtos.Order;
using Furniture.Core.Dtos.Product;
using Microsoft.AspNetCore.Mvc;

namespace Furniture.API.Controllers;

public class OrderController(IOrderService _service, IFileStorageService _blob) : BaseApiController
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

	[HttpPost("blob")]
	public async Task<string> AddBlob(IFormFile image)
	{
		var file = await _blob.UploadFileAsync("product", image);
		return file;
	}
}
