namespace Furniture.Infrastructure.Repositories.Interfaces;

public interface ICartItemRepository : IGenericRepository<CartItem>
{
    Task<List<CartItem>> GetAllAsync(Guid cartId);
}
