namespace Furniture.Service;

public interface IOrderService
{
	Task<bool> CreateOrderAsync(CreateOrderDto model);
	Task<List<OrderItemDto>> GetOrdersAsync();
	Task<bool> ChangeStatusAsync(Guid orderId, string roleName);
}
