namespace Furniture.Infrastructure;

public interface IOrderItemRepository : IGenericRepository<OrderItem>
{
	Task AddRangeAsync(List<OrderItem> domain);
}
