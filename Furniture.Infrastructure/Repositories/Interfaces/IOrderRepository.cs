using Furniture.Core.Dtos.Order;

namespace Furniture.Infrastructure.Repositories.Interfaces;

public interface IOrderRepository : IGenericRepository<Order>
{
	Task<List<OrderItemDto>> GetOrdersAsync(Guid id);
}
