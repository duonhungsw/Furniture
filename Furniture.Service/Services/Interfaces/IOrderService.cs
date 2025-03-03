namespace Furniture.Service;

public interface IOrderService
{
	Task<bool> CreateOrderAsync(CreateOrderDto model);
	Task<List<OrderItemDto>> GetOrdersAsync();
}
