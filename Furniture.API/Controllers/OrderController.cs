using Furniture.Core.Dtos.Order;
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
	[HttpPost("blob")]
	public async Task<string> AddBlob(IFormFile image)
	{
		var file = await _blob.UploadFileAsync("product", image);
		return file;
	}
}
