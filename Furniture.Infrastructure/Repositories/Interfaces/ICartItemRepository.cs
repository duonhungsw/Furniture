namespace Furniture.Infrastructure;

public interface ICartItemRepository : IGenericRepository<CartItem>
{
	Task AddCartItemAsync(CartItem cartItem);
	Task<bool> AddCartItemIsContainAsync(CartItem cartItem, int quantity);
	Task<bool> CheckCartItemByProductIdAsync(Cart cart, Guid ProductId);
	Task<CartItem?> GetCartItemByCartIdAndProductIdAsync(Guid cartId, Guid productId);
	Task<List<CartItem>?> GetCartItemByUserIdAsync(Guid userId);
	Task<bool> IsCartItemExistsAsync(Guid productId, Guid accountId);
	Task<List<CartItem>?> GetCartsItemByCartIdAndProductIdAsync(Guid cartId, Guid productId);
	void DeleteRangeAsync(List<CartItem> cartItems);

}
