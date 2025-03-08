namespace Furniture.Web.Services;

public interface IOrderApi
{
	[Get("/orders/checkout")]
	Task<List<OrderCheckout>> GetOrdersForAccount(Guid id);
	[Post("/orders/create")]
	Task<bool> CreateOrder([Body] CreateOrderDto model);
}
