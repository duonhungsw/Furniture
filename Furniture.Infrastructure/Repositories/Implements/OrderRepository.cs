namespace Furniture.Infrastructure;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
	public OrderRepository(ApplicationDbContext context) : base(context)
	{

	}
	public async Task<List<OrderItemDto>> GetOrdersAsync(Guid id)
	{
		var entities = await (from order in appDbContext.Orders
							  join orderItems in appDbContext.OrderItems on order.Id equals orderItems.OrderId
							  where order.AccountId == id
							  select new OrderItemDto
							  {
								  Id = orderItems.Id,
								  OrderId = order.Id,
								  Order = order != null ? new OrderDto
								  {
									  TotalMoney = order.TotalMoney,
								  } : null,
								  ProductId = orderItems.ProductId,
								  Product = orderItems.Product != null ? new ProductDto
								  {
									  Id = orderItems.ProductId,
									  Name = orderItems.Product.Name,
									  PictureUrl = First(orderItems.Product.PictureUrl),
									  Price = orderItems.Product.Price
								  } : null,
								  Quantity = orderItems.Quantity
							  }).ToListAsync();

		return entities;
	}
	private static string First(string s)
	{
		return s.Split(',').FirstOrDefault()!;
	}

	public async Task<Status?> GetStatusByNameAsync(string statusName)
		=> await appDbContext.Statuses.AsNoTracking().FirstOrDefaultAsync(s => s.Name == statusName);

	public async Task<List<OrderCheckout>> GetOrdersForAccountAsync(Guid id)
	{
		var entities = await (from cartItem in appDbContext.CartItems
							  join cart in appDbContext.Carts on cartItem.CartId equals cart.Id
							  where cart.AccountId == id && cartItem.Status == true
							  select new OrderCheckout
							  {
								  ProductId = cartItem.ProductId,
								  ProductName = cartItem.Product == null ? null : cartItem.Product.Name,
								  Price = cartItem.Price,
								  TotalMoney = cartItem.Price * cartItem.Quantity
							  }).ToListAsync();
		return entities;
	}
}
