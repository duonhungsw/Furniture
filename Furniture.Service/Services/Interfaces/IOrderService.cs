using Furniture.Core.Dtos.Order;
using Microsoft.AspNetCore.Mvc;

namespace Furniture.Service;

public interface IOrderService
{
	Task<bool> CreateOrderAsync(CreateOrderDto model);
	Task<List<OrderDto>> GetOrdersAsync(Guid accountId, QueryInfo queryInfo, Guid statusId);
	Task<bool> ChangeStatusAsync(Guid orderId, string roleName);
	Task<List<OrderCheckout>> GetOrdersForAccountAsync(Guid id);
    Task<MonthlyRevenueViewModel> GetMonthlyRevenue();
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    Task<bool> UpdateOrderStatusAsync(Guid orderId, Guid statusId);
}
