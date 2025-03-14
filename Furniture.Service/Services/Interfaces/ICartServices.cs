namespace Furniture.Service;

public interface ICartServices
{
    Task<List<CartItemDto>?> GetCartsAsync(Guid accountId);
    Task<bool> DeleteCartItemAsync(Guid cartItemID);
    Task<bool> UpdateCartItemByQuantityAsync(Guid cartItemId, int quantity);
    Task<bool> UpdateCartItemByStatusAsync(Guid cartItemId);
    Task<bool> AddCartItemAsync(CartAddDto model, Guid accountId);

}