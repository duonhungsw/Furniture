using Furniture.Core.Dtos.Order;

namespace Furniture.Infrastructure;

public interface IOrderRepository : IGenericRepository<Order>
{
	Task<List<OrderDto>> GetOrdersAsync(Guid id, QueryInfo queryInfo, Guid statusId);
	Task<List<OrderCheckout>> GetOrdersForAccountAsync(Guid id);
    Task<List<MonthlyRevenueDto>> GetMonthlyRevenue();
    Task<bool> UpdateOrderStatusAsync(Guid orderId, Guid statusId);
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
}
