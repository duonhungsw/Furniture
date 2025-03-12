namespace Furniture.Infrastructure;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
	public OrderRepository(ApplicationDbContext context) : base(context)
	{

	}
	public async Task<List<OrderDto>> GetOrdersAsync(Guid id, QueryInfo queryInfo, Guid statusId)
	{
		var entities = await (from order in appDbContext.Orders.AsNoTracking()
							  join orderItems in appDbContext.OrderItems.AsNoTracking() on order.Id equals orderItems.OrderId
							  join products in appDbContext.Products.AsNoTracking() on orderItems.ProductId equals products.Id
							  join statuses in appDbContext.Statuses.AsNoTracking() on order.StatusId equals statuses.Id
							  where order.AccountId == id
									&& (statusId == Guid.Empty || order.StatusId == statusId)
									//&& (queryInfo.SearchText == string.Empty || order.Id == Guid.Parse(queryInfo.SearchText!))
							  orderby order.LastModified descending
							  group new { order, orderItems, products, statuses } by order into grouped
							  select new OrderDto
							  {
								  Id = grouped.Key.Id,
								  Address = grouped.Key.Detail + ", " + grouped.Key.Town + ", " + grouped.Key.District + ", " + grouped.Key.City + ", " + grouped.Key.Country,
								  TotalMoney = grouped.Key.TotalMoney,
								  PaymentMethod = grouped.Key.PaymentMethod,
								  CreateAt = grouped.Key.CreatedAt!.Value.ToString("dd-MM-yyyy"),
								  StatusId = grouped.Key.StatusId,
								  Status = grouped.Key.StatusId != null ? new StatusDto
								  {
									  Id = grouped.Key.StatusId,
									  Name = grouped.First().statuses != null ? grouped.First().statuses.Name : "Unknown"
								  } : null,
								  OrderItems = grouped.Select(g => new CreateOrderItemDto
								  {
									  ProductId = g.products.Id,
									  Product = new ProductDto
									  {
										  Name = g.products.Name,
										  PictureUrl = First(g.products.PictureUrl),
									  },
									  Price = g.products.Price,
									  Quantity = g.orderItems.Quantity
								  }).ToList()
							  }).ToListAsync();

		return entities;
	}

	private static string First(string s)
	{
		return s.Split(',').FirstOrDefault()!;
	}

	public async Task<List<OrderCheckout>> GetOrdersForAccountAsync(Guid id)
	{
		var entities = await (from cartItem in appDbContext.CartItems.AsNoTracking()
							  join cart in appDbContext.Carts.AsNoTracking() on cartItem.CartId equals cart.Id
							  where cart.AccountId == id && cartItem.Status == true
							  select new OrderCheckout
							  {
								  ProductId = cartItem.ProductId,
								  ProductName = cartItem.Product == null ? null : cartItem.Product.Name,
								  Quantity = cartItem.Quantity,
								  Price = cartItem.Price,
								  TotalMoney = cartItem.Price * cartItem.Quantity
							  }).ToListAsync();
		return entities;
	}
}
