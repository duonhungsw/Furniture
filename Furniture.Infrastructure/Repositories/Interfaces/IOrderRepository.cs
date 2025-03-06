namespace Furniture.Infrastructure;

public interface IOrderRepository : IGenericRepository<Order>
{
	Task<List<OrderItemDto>> GetOrdersAsync(Guid id);
	Task<Status?> GetStatusByNameAsync(string statusName);
	Task<List<OrderCheckout>> GetOrdersForAccountAsync(Guid id);
}
