using Furniture.Core.Dtos.Cart;

namespace Furniture.Infrastructure.Repositories.Interfaces;

public interface ICartRepository : IGenericRepository<Cart>
{
    Task<List<Cart>> GetCartsAsync(Guid accountId);
    Task<List<CartItemDto>> GetCartProductsAsync(Guid accountId);
    Task<bool> DeleteCartItem(Guid accountId,Guid cartItemID);
    Task DeleteCartItem(Guid cartItemID);
    Task<bool> UpdateCartItemByQuantity(Guid accountId, Guid cartItemID, int quantity);
    Task UpdateCartItemByQuantity(Guid cartItemID, int quantity);
    Task<bool> UpdateCartItemByStatus(Guid accountId, Guid cartItemID);
    Task UpdateCartItemByStatus(Guid cartItemID);
    Task AddCartItemAsync(CartItem cartItem);
    Task<Cart?> GetCartByAccountId(Guid accountId);

}
