namespace Furniture.Infrastructure;

public interface ICartRepository : IGenericRepository<Cart>
{
	Task<List<Cart>> GetCartsAsync(Guid accountId);
	Task<List<CartItemDto>> GetCartProductsAsync(Guid accountId);
	Task AddCartItemAsync(CartItem cartItem);

}
