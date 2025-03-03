using Furniture.Core.Dtos.Cart;
using Microsoft.AspNetCore.Mvc;

namespace Furniture.API.Controllers;

public class CartController(ICartServices cartServices
                            ) : BaseApiController
{
    [HttpGet("/Cart/ShoppingCart")]
    public async Task<List<CartItemDto>?> GetCarts()
    {
        return await cartServices.GetCartsAsync();
    }
    [HttpDelete("{cartItemId}")]
    public async Task<bool> DeleteCartItem([FromRoute]Guid cartItemId)
    {
        return await cartServices.DeleteCartItemAsync(cartItemId);
    }
    [HttpPatch("update/quantity")]
    public async Task<bool> UpdateCartItemByQuantity([FromBody] CartUpdateQuantityDto model)
    {
        return await cartServices.UpdateCartItemByQuantityAsync(model.CartId,model.Quantity);
    }
    [HttpPatch("update/status/{cartItemId}")]
    public async Task<bool> UpdateCartItemByStatus([FromRoute]Guid cartItemId)
    {
        return await cartServices.UpdateCartItemByStatusAsync(cartItemId);
    }
    [HttpPost("add")]
    public async Task<bool> AddCartItem([FromForm]CartAddDto model)
    {
        return await cartServices.AddCartItemAsync(model);
    }
}
