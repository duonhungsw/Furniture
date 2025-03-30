namespace Furniture.Infrastructure;

public interface IOrderItemRepository : IGenericRepository<OrderItem>
{
	Task AddRangeAsync(IEnumerable<OrderItem> domain);
}
