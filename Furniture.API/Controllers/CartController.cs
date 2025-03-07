namespace Furniture.API.Controllers;
[Route("carts")]
public class CartController(ICartServices cartServices
                            ) : BaseApiController
{
    [HttpGet("ShoppingCart")]
    public async Task<List<CartItemDto>?> GetCarts()
    {
        return await cartServices.GetCartsAsync();
    }
    [HttpDelete("{cartItemId}")]
    public async Task<bool> DeleteCartItem([FromRoute] Guid cartItemId)
    {
        return await cartServices.DeleteCartItemAsync(cartItemId);
    }
    [HttpPatch("update/quantity")]
    public async Task<IActionResult> UpdateCartItemByQuantity([FromBody] CartUpdateQuantityDto model)
    {
        var isUpdated = await cartServices.UpdateCartItemByQuantityAsync(model.CartItemId, model.Quantity);
        if (isUpdated)
        {
            return Ok(new { message = "Cập nhật thành công!" });
        }
        return BadRequest(new { message = "Cập nhật thất bại!" });
    }
    [HttpPatch("update/status/{cartItemId}")]
    public async Task<bool> UpdateCartItemByStatus([FromRoute] Guid cartItemId)
    {
        return await cartServices.UpdateCartItemByStatusAsync(cartItemId);
    }
    [HttpPost("add")]
    public async Task<bool> AddCartItem([FromForm] CartAddDto model)
    {
        return await cartServices.AddCartItemAsync(model);
    }

    
}
