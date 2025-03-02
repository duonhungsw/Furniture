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
        return await cartServices.DeleteCartItem(cartItemId);
    }
    [HttpPatch("update/quantity")]
    public async Task<bool> UpdateCartItemByQuantity([FromBody] CartUpdateQuantityDto model)
    {
        return await cartServices.UpdateCartItemByQuantity(model.CartId,model.Quantity);
    }
    [HttpPatch("update/status/{cartItemId}")]
    public async Task<bool> UpdateCartItemByStatus([FromRoute]Guid cartItemId)
    {
        return await cartServices.UpdateCartItemByStatus(cartItemId);
    }
    [HttpPost("add")]
    public async Task<bool> AddCartItem([FromForm]CartAddDto model)
    {
        return await cartServices.AddCartItem(model);
    }
}
