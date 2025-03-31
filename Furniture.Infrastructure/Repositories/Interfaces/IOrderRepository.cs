using Furniture.Core.Dtos.Order;
using Microsoft.EntityFrameworkCore.Storage;

namespace Furniture.Infrastructure;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<List<OrderDto>> GetOrdersAsync(Guid id, QueryInfo queryInfo, Guid statusId);
    Task<List<OrderCheckout>> GetOrdersForAccountAsync(Guid id);
    Task<List<MonthlyRevenueDto>> GetMonthlyRevenue();
    Task<bool> UpdateOrderStatusAsync(Guid orderId, Guid statusId); 
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task<bool> IsProductUsedInOrdersAsync(Guid productId);
}
