
namespace Furniture.Infrastructure.Repositories.Implements;

public class OrderItemRepository : GenericRepository<OrderItem>, IOrderItemRepository
{
	public OrderItemRepository(ApplicationDbContext context) : base(context)
	{

	}
	public async Task AddRangeAsync(List<OrderItem> domain)
	{
		 await appDbContext.OrderItems.AddRangeAsync(domain);
	}
}
