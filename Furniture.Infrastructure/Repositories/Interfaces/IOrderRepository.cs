namespace Furniture.Infrastructure;

public interface IOrderRepository : IGenericRepository<Order>
{
	Task<List<OrderDto>> GetOrdersAsync(Guid id, QueryInfo queryInfo, Guid statusId);
	Task<List<OrderCheckout>> GetOrdersForAccountAsync(Guid id);
}
