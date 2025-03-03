namespace Furniture.Infrastructure;

public interface IOrderRepository : IGenericRepository<Order>
{
	Task<List<OrderItemDto>> GetOrdersAsync(Guid id);
}
