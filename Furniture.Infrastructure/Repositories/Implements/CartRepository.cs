using Microsoft.EntityFrameworkCore;

namespace Furniture.Infrastructure;

public class CartRepository : GenericRepository<Cart>, ICartRepository
{
	public CartRepository(ApplicationDbContext context) : base(context)
	{
	}

	public async Task<List<Cart>> GetCartsAsync(Guid accountId)
	{
		var entities = await appDbContext.Carts
			.AsNoTracking()
			.Where(c => c.AccountId == accountId)
			.ToListAsync();
		return entities;
	}
	public async Task<List<CartItemDto>> GetCartProductsAsync(Guid accountId)
	{
		{
			var cartProducts = await (from cart in appDbContext.Carts
									  join cartItem in appDbContext.CartItems on cart.Id equals cartItem.CartId
									  join product in appDbContext.Products on cartItem.ProductId equals product.Id
									  where cart.AccountId == accountId
									  select new CartItemDto
									  {
										  Id = cartItem.Id,
										  ProductId = product.Id,
										  ProductName = product.Name,
										  Quatity = cartItem.Quantity,
										  ProductPrice = cartItem.Price,
										  UrlImage = product.PictureUrl,
										  Status = cartItem.Status,
										  TotalMoney = cartItem.TotalMoney
									  }).ToListAsync();

			return cartProducts;
		}
	}
	public async Task<bool> DeleteCartItemAsync(Guid accountId, Guid cartItemID)
	{
		var listCartItems = await GetCartProductsAsync(accountId);
		foreach (var cartItem in listCartItems)
		{
			if (cartItem.Id.Equals(cartItemID))
			{
				await DeleteCartItemAsync(cartItemID);
				return true;
#pragma warning disable CS0162 // Unreachable code detected
				break;
#pragma warning restore CS0162 // Unreachable code detected
			}
		}
		return false;
	}
	public async Task DeleteCartItemAsync(Guid cartItemID)
	{
		var cartItem = await appDbContext.CartItems.FirstOrDefaultAsync(c => c.Id.Equals(cartItemID));
		if (cartItem != null)
		{
			appDbContext.CartItems.Remove(cartItem);
			appDbContext.SaveChanges();
			await appDbContext.SaveChangesAsync();
		}
	}
	public async Task<bool> UpdateCartItemByQuantityAsync(Guid accountId, Guid cartItemID, int quantity)
	{
		var listCartItems = await GetCartProductsAsync(accountId);
		foreach (var cartItem in listCartItems)
		{
			if (cartItem.Id.Equals(cartItemID))
			{
				cartItem.Quatity = quantity;
				cartItem.TotalMoney = quantity * cartItem.ProductPrice;
				await appDbContext.SaveChangesAsync();
				return true;
			}
		}
		return false;
	}
	public async Task UpdateCartItemByStatusAsync(Guid cartItemID)
	{
		var cartItem = await appDbContext.CartItems.FindAsync(cartItemID);
		if (cartItem != null)
		{
			if (cartItem.Status)
			{
				cartItem.Status = false;
				await appDbContext.SaveChangesAsync();
			}
			else if (!cartItem.Status)
			{
				cartItem.Status = true;
				await appDbContext.SaveChangesAsync();
			}

		}
	}
	public async Task<bool> UpdateCartItemByStatusAsync(Guid accountId, Guid cartItemID)
	{
		var listCartItems = await GetCartProductsAsync(accountId);
		foreach (var cartItem in listCartItems)
		{
			if (cartItem.Id.Equals(cartItemID))
			{
				await UpdateCartItemByStatusAsync(cartItemID);
				return true;
#pragma warning disable CS0162 // Unreachable code detected
				break;
#pragma warning restore CS0162 // Unreachable code detected
			}
			else
			{
				return false;
			}
		}
		return false;
	}
	public async Task AddCartItemAsync(CartItem cartItem)
	{
		appDbContext.CartItems.Add(cartItem);
		await appDbContext.SaveChangesAsync();
	}
	public async Task<Cart?> GetCartByAccountIdAsync(Guid accountId)
	{
		return await appDbContext.Carts.FirstOrDefaultAsync(c => c.AccountId == accountId);
	}



}
