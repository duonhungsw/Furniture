using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Furniture.Infrastructure.Repositories.Implements
{
    public class CartItemRepositoty : GenericRepository<CartItem>, ICartItemRepository
    {
        public CartItemRepositoty(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<CartItem>> GetAllAsync(Guid cartId)
        {
            var entities = await appDbContext.CartItems
                .AsNoTracking()
                .Where(c =>c.CartId == cartId)
                .Select(c => new CartItem 
                {
                    Id = c.Id,
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    Price = c.Price,

                }).ToListAsync();
            return entities;
        }
    }
}
