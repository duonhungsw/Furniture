namespace Furniture.API.Controllers;
[Route("carts")]
public class CartController(ICartServices cartServices
                            ) : BaseApiController
{
    [HttpGet("{accountId}/ShoppingCart")]
    public async Task<List<CartItemDto>?> GetCarts([FromRoute] Guid accountId )
    {
        return await cartServices.GetCartsAsync(accountId);
    }
    [HttpDelete("{cartItemId}")]
    public async Task<bool> DeleteCartItem([FromRoute] Guid cartItemId)
    {
        return await cartServices.DeleteCartItemAsync(cartItemId);
    }
    [HttpPatch("update/quantity")]
    public async Task<bool> UpdateCartItemByQuantity([FromBody] CartUpdateQuantityDto model)
    {
        return await cartServices.UpdateCartItemByQuantityAsync(model.CartItemId, model.Quantity);
       
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