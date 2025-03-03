using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Furniture.Infrastructure.Repositories.Interfaces
{
    public interface ICartItemRepository : IGenericRepository<CartItem>
    {
        Task AddCartItemAsync(CartItem cartItem);
        Task<bool> AddCartItemIsContainAsync(CartItem cartItem, int quantity);
        Task<bool> CheckCartItemByProductIdAsync(Cart cart,Guid ProductId);
        Task<CartItem?> GetCartItemByCartIdAndProductIdAsync(Guid cartId, Guid productId);
    }
}
