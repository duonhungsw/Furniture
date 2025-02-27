
using Furniture.Core.Dtos.Cart;

namespace Furniture.Service.Services.Implements;

public class CartServices(ICartRepository cartRepository, ITokenService tokenService) : ICartServices
{
    public async Task<List<CartItemDto>> GetCartsAsync()
    {

        var account = await tokenService.Authenticate();
        if (account != null)
        {
            return await cartRepository.GetCartProductsAsync(account.Id);
        }
        return null;
    }
}
