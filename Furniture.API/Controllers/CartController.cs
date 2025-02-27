using Furniture.Core.Dtos.Cart;
using Microsoft.AspNetCore.Mvc;

namespace Furniture.API.Controllers;

public class CartController(ICartServices cartServices
                            ) : BaseApiController
{
    [HttpGet("/Cart/ShoppingCart")]
    public async Task<List<CartItemDto>> GetCarts()
    {
        return await cartServices.GetCartsAsync();
    }
}
