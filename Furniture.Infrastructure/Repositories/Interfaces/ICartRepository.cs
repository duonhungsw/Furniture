using Furniture.Core.Dtos.Cart;

namespace Furniture.Infrastructure.Repositories.Interfaces;

public interface ICartRepository : IGenericRepository<Cart>
{
    Task<List<Cart>> GetCartsAsync(Guid accountId);
    Task<List<CartItemDto>> GetCartProductsAsync(Guid accountId);
}
