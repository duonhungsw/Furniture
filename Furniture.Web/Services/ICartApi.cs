namespace Furniture.Web.Services;
public interface ICartApi
{
    [Get("/carts/ShoppingCart")]
    Task<ApiResponse<List<CartItemDto>?>> GetCarts();
    [Delete("/carts/{cartItemId}")]
    Task<bool> DeleteCartItem([FromRoute] Guid cartItemId);
    [Patch("/carts/update/quantity")]
    Task<bool> UpdateCartItemByQuantity([FromBody] CartUpdateQuantityDto model);
    [Patch("/carts/update/status/{cartItemId}")]
    Task<bool> UpdateCartItemByStatus([FromRoute] Guid cartItemId);
    [Post("/carts/add")]
    Task<bool> AddCartItem([FromForm] CartAddDto model);
}
