using Furniture.Core.Dtos.Order;

namespace Furniture.Service.Services.Interfaces;

public interface IOrderService
{
	Task<bool> CreateOrderAsync(CreateOrderDto model);
}
