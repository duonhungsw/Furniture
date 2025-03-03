namespace Furniture.Infrastructure;

public interface ICartItemRepository : IGenericRepository<CartItem>
{
	Task AddCartItemAsync(CartItem cartItem);
	Task<bool> AddCartItemIsContainAsync(CartItem cartItem, int quantity);
	Task<bool> CheckCartItemByProductIdAsync(Cart cart, Guid ProductId);
	Task<CartItem?> GetCartItemByCartIdAndProductIdAsync(Guid cartId, Guid productId);
}
