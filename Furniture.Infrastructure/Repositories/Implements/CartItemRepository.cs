using Microsoft.EntityFrameworkCore;

namespace Furniture.Infrastructure.Repositories.Implements;

public class CartItemRepository : GenericRepository<CartItem>, ICartItemRepository
{
    public CartItemRepository(ApplicationDbContext context) : base(context)
    {

    }
    public async Task AddCartItem(CartItem cartItem)
    {
        await appDbContext.CartItems.AddAsync(cartItem); // Thêm vào DB theo cách async
        await appDbContext.SaveChangesAsync(); // Lưu thay đổi vào DB
    }
    public async Task<bool> AddCartItemIsContain(CartItem cartItem, int quantity)
    {
        var existingCartItem = await appDbContext.CartItems.FindAsync(cartItem.Id);

        if (existingCartItem == null)
        {
            return false; // Không tìm thấy sản phẩm trong giỏ hàng
        }

        existingCartItem.Quantity += quantity; // Tăng số lượng lên 1

        await appDbContext.SaveChangesAsync(); // EF Core tự động theo dõi thay đổi
        return true; // Trả về true nếu cập nhật thành công
    }
    public async Task<bool> CheckCartItemByProductId(Cart cart, Guid ProductId)
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
    public async Task<CartItem?> GetCartItemByCartIdAndProductId(Guid cartId,Guid productId)
    {
        var cartItem = new CartItem();
        var cartItemList = await appDbContext.CartItems.ToListAsync();
        foreach (var item in cartItemList)
        {
            if(item.CartId == cartId && item.ProductId == productId)
            {
                cartItem = item;
                return cartItem;
            }
        }
        return null;
    }
}
