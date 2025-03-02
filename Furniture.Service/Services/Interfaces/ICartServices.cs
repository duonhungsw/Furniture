using Furniture.Core.Dtos.Cart;

namespace Furniture.Service.Services.Interfaces;

public interface ICartServices
{
    Task<List<CartItemDto>?> GetCartsAsync();
    Task<bool> DeleteCartItem(Guid cartItemID);
    Task<bool> UpdateCartItemByQuantity(Guid cartItemId, int quantity);
    Task<bool> UpdateCartItemByStatus(Guid cartItemId);
    Task<bool> AddCartItem(CartAddDto model);
    Task<Cart> GetCartByAccountId();
}
