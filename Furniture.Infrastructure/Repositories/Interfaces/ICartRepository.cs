namespace Furniture.Infrastructure;

public interface ICartRepository : IGenericRepository<Cart>
{
	Task<List<Cart>> GetCartsAsync(Guid accountId);
	Task<List<CartItemDto>> GetCartProductsAsync(Guid accountId);
	Task<bool> DeleteCartItemAsync(Guid accountId, Guid cartItemID);
	Task DeleteCartItemAsync(Guid cartItemID);
	Task<bool> UpdateCartItemByQuantityAsync(Guid accountId, Guid cartItemID, int quantity);
	Task<bool> UpdateCartItemByStatusAsync(Guid accountId, Guid cartItemID);
	Task UpdateCartItemByStatusAsync(Guid cartItemID);
	Task AddCartItemAsync(CartItem cartItem);
	Task<Cart?> GetCartByAccountIdAsync(Guid accountId);

}
