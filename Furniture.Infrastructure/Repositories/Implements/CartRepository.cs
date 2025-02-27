using Furniture.Core.Dtos.Cart;
using Microsoft.EntityFrameworkCore;

namespace Furniture.Infrastructure.Repositories.Implements;

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
                                          ProductQuatity = cartItem.Quantity,
                                          ProductPrice = cartItem.Price,
                                          UrlImage = product.PictureUrl,
                                          Status = cartItem.Status,
                                      }).ToListAsync();

            return cartProducts;
        }
    }

}
