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
										  Quantity = cartItem.Quantity,
										  Price = cartItem.Price,
										  UrlImage = product.PictureUrl,
										  Status = cartItem.Status,
										  TotalMoney = cartItem.TotalMoney,
										  CartId = product.Id,
										  QuantityInStock = product.QuantityInStock
									  }).ToListAsync();

			return cartProducts;
		}
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
