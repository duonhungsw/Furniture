using Microsoft.EntityFrameworkCore;

namespace Furniture.Infrastructure;

public class CartItemRepository : GenericRepository<CartItem>, ICartItemRepository
{
	public CartItemRepository(ApplicationDbContext context) : base(context)
	{

	}
	public async Task AddCartItemAsync(CartItem cartItem)
	{
		await appDbContext.CartItems.AddAsync(cartItem); // Thêm vào DB theo cách async
		await appDbContext.SaveChangesAsync(); // Lưu thay đổi vào DB
	}
	public async Task<bool> AddCartItemIsContainAsync(CartItem cartItem, int quantity)
	{
		var existingCartItem = await appDbContext.CartItems.FindAsync(cartItem.Id);

		if (existingCartItem == null)
		{
			return false; // Không tìm thấy sản phẩm trong giỏ hàng
		}

		existingCartItem.Quantity += quantity; 

		await appDbContext.SaveChangesAsync(); // EF Core tự động theo dõi thay đổi
		return true; // Trả về true nếu cập nhật thành công
	}
	public async Task<bool> CheckCartItemByProductIdAsync(Cart cart, Guid ProductId)
	{
		var listCartItemWithCartId = await appDbContext.CartItems.ToListAsync();
		List<CartItem> cartItems = new List<CartItem>();
		foreach (var _cartItem in listCartItemWithCartId)
		{
			if (_cartItem.CartId == cart.Id)
			{
				cartItems.Add(_cartItem);
			}
		}
		foreach (var cartItem in cartItems)
		{
			if (cartItem.ProductId == ProductId)
			{
				return true;
			}
		}
		return false;
	}
	public async Task<CartItem?> GetCartItemByCartIdAndProductIdAsync(Guid cartId, Guid productId)
	{
		var cartItem = new CartItem();
		var cartItemList = await appDbContext.CartItems.ToListAsync();
		foreach (var item in cartItemList)
		{
			if (item.CartId == cartId && item.ProductId == productId)
			{
				cartItem = item;
				return cartItem;
			}
		}
		return null;
	}
    public async Task<List<CartItem>?> GetCartItemByUserIdAsync(Guid userId)
	{
		var cart = await appDbContext.Carts.FirstOrDefaultAsync(c => c.AccountId == userId);
        return await appDbContext.CartItems.Where(c => c.CartId == cart.Id).ToListAsync();
    }
    public async Task<bool> IsCartItemExistsAsync(Guid productId, Guid accountId)
    {
        var cart = await appDbContext.Carts.FirstOrDefaultAsync(c => c.AccountId == accountId);
        var cartItem = await appDbContext.CartItems
            .FirstOrDefaultAsync(ci => ci.ProductId == productId && ci.CartId == cart.Id);

        return cartItem != null;
    }

}
