using Furniture.Core.Dtos.Order;

namespace Furniture.Web.Services;

public interface IOrderApi
{
	[Get("/orders/checkout")]
	Task<List<OrderCheckout>> GetOrdersForAccount(Guid id);
	[Post("/orders/create")]
	Task<bool> CreateOrder([Body] CreateOrderDto model);
	[Get("/orders/{accountId}/purchase")]
	Task<List<OrderDto>> GetPurchases(Guid accountId, [Query] Guid? statusId, [Query] QueryInfo queryInfo);
    [Get("/orders/revenue")]
    Task<MonthlyRevenueViewModel> GetMonthlyRevenue();
    [Get("/orders")]
    Task<PagedResult<OrderDto>> GetAllOrdersAsync([Query] QueryInfo queryInfo);
    [Put("/status/{statusId}")]
    Task<bool> UpdateOrderStatus(Guid orderId, Guid statusId);
}
