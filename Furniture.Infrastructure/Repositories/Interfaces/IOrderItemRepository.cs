namespace Furniture.Infrastructure.Repositories.Interfaces;

public interface IOrderItemRepository : IGenericRepository<OrderItem>
{
	Task AddRangeAsync(List<OrderItem> domain);
}
