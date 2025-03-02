using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Furniture.Infrastructure.Repositories.Interfaces
{
    public interface ICartItemRepository : IGenericRepository<CartItem>
    {
        Task AddCartItem(CartItem cartItem);
        Task<bool> AddCartItemIsContain(CartItem cartItem, int quantity);
        Task<bool> CheckCartItemByProductId(Cart cart,Guid ProductId);
        Task<CartItem?> GetCartItemByCartIdAndProductId(Guid cartId, Guid productId);
    }
}
