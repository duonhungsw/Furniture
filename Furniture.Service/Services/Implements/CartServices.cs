namespace Furniture.Service;

public class CartServices(
	ICartRepository cartRepository, 
	IProductRepository productRepository, 
	ITokenService tokenService, 
	IMapper mapper, 
	ICartItemRepository cartItemRepository) : ICartServices
{
	public async Task<List<CartItemDto>?> GetCartsAsync()
	{

		var account = await tokenService.Authenticate();
		if (account != null)
		{
			return await cartRepository.GetCartProductsAsync(account.Id);
		}
		return null;
	}
	public async Task<bool> DeleteCartItemAsync(Guid cartItemID)
	{
		var account = await tokenService.Authenticate();
		if (account != null)
		{
			var checkDelelte = await cartRepository.DeleteCartItemAsync(account.Id, cartItemID);
			if (checkDelelte)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		return false;
	}
	public async Task<bool> UpdateCartItemByQuantityAsync(Guid cartItemId, int quantity)
	{
		var account = await tokenService.Authenticate();
		if (account != null)
		{
			var checkUpdate = await cartRepository.UpdateCartItemByQuantityAsync(account.Id, cartItemId, quantity);
			if (checkUpdate)
			{
				return true;
			}
			else
			{
				return false;
			}


		}
		return false;
	}
	public async Task<bool> UpdateCartItemByStatusAsync(Guid cartItemId)
	{
		var account = await tokenService.Authenticate();
		if (account != null)
		{
			var checkUpdate = await cartRepository.UpdateCartItemByStatusAsync(account.Id, cartItemId);
			if (checkUpdate)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		return false;
	}
	public async Task<bool> AddCartItemAsync(CartAddDto model)
	{
		Guid productId = model.ProductId;
		int quantity = model.Quantity;
		var account = await tokenService.Authenticate();
		if (account != null)
		{
			var cartByAccountId = await cartRepository.GetCartByAccountIdAsync(account.Id);
			var productByProductId = await productRepository.GetByIdAsync(productId);
			if (await cartItemRepository.CheckCartItemByProductIdAsync(cartByAccountId, productByProductId.Id))
			{
				var cartItem = await cartItemRepository.GetCartItemByCartIdAndProductIdAsync(cartByAccountId.Id, productId);
				await cartItemRepository.AddCartItemIsContainAsync(cartItem, quantity);
			}
			else
			{
				var cartItem = new CartItem();
				cartItem.ProductId = productId;
				cartItem.CartId = cartByAccountId.Id;
				cartItem.Quantity = quantity;
				cartItem.Status = false;
				cartItem.Price = productByProductId.Price;
				await cartItemRepository.AddCartItemAsync(cartItem);
				return true;
			}
		}
		return false;
	}
	public async Task<Cart> GetCartByAccountIdAsync()
	{
		var account = await tokenService.Authenticate();
		if (account != null)
		{
			var cart = await cartRepository.GetCartByAccountIdAsync(account.Id);
			return cart;
		}
		return null;
	}
}