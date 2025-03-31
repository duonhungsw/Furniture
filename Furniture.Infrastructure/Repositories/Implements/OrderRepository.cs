using Furniture.Core.Dtos.Order;
using Microsoft.EntityFrameworkCore.Storage;

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
	public async Task<List<MonthlyRevenueDto>> GetMonthlyRevenue()
	{
		return await appDbContext.Orders
			.Where(o => o.CreatedAt.HasValue)
			.GroupBy(o => new { Year = o.CreatedAt.Value.Year, Month = o.CreatedAt.Value.Month })
			.Select(g => new MonthlyRevenueDto
			{
				Year = g.Key.Year,
				Month = g.Key.Month,
				TotalRevenue = g.Sum(o => o.TotalMoney)
			})
			.ToListAsync();
	}
	public async Task<bool> UpdateOrderStatusAsync(Guid orderId, Guid statusId)
	{
		var order = await appDbContext.Orders.FindAsync(orderId);
		if (order == null) return false;

		order.StatusId = statusId;
		await appDbContext.SaveChangesAsync();
		return true;
	}
	public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
	{
		var orders = await (from o in appDbContext.Orders
							join a in appDbContext.Accounts on o.AccountId equals a.Id
							select new OrderDto
							{
								Id = o.Id,
								AccountId = o.AccountId,
								Account = new AccountDto { Name = a.Name },
								Address = $"{o.Detail}, {o.Town}, {o.District}, {o.City}, {o.Country}",
								CreateAt = o.CreatedAt.HasValue ? o.CreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
								Phone = o.Phone,
								Note = o.Note,
								TotalMoney = o.TotalMoney,
								PaymentMethod = o.PaymentMethod,
								StatusId = o.StatusId,
								Status = new StatusDto
								{
									Id = o.Status.Id,
									Name = o.Status.Name
								},
								OrderItems = o.OrderItems.Select(oi => new CreateOrderItemDto
								{
									OrderId = oi.OrderId,
									ProductId = oi.ProductId,
									Product = oi.Product != null ? new ProductDto
									{
										Id = oi.Product.Id,
										Name = oi.Product.Name,
										Price = oi.Product.Price
									} : null,
									Quantity = oi.Quantity,
									Price = oi.Product.Price
								}).ToList()
							})
							.AsNoTracking()
							.ToListAsync();

		return orders;
	}
	public async Task<IDbContextTransaction> BeginTransactionAsync()
	{
		return await appDbContext.Database.BeginTransactionAsync();
	}
    public async Task<bool> IsProductUsedInOrdersAsync(Guid productId)
    {
        return await appDbContext.OrderItems.AnyAsync(oi => oi.ProductId == productId);
    }
}
