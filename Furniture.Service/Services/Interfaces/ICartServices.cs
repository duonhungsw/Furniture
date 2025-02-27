using Furniture.Core.Dtos.Cart;

namespace Furniture.Service.Services.Interfaces;

public interface ICartServices
{
    Task<List<CartItemDto>> GetCartsAsync();
}
