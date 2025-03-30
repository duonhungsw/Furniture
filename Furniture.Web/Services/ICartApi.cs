namespace Furniture.Web.Services;
public interface ICartApi
{
    [Get("/carts/{accountId}/ShoppingCart")]
    Task<ApiResponse<List<CartItemDto>?>> GetCarts(Guid accountId);
    [Delete("/carts/{cartItemId}")]
    Task<bool> DeleteCartItem([FromRoute] Guid cartItemId);
    [Patch("/carts/update/quantity")]
    Task<bool> UpdateCartItemByQuantity([FromBody] CartUpdateQuantityDto model);
    [Patch("/carts/update/status/{cartItemId}")]
    Task<bool> UpdateCartItemByStatus([FromRoute] Guid cartItemId);
    [Post("/carts/{accountId}/add")]
    Task<bool> AddCartItem([Body] CartAddDto model, [AliasAs("accountId")] Guid accountId);
    [Get("/carts/{accountId}/items-number")]
    Task<int> GetCartItemsNumber([FromRoute] Guid accountId);
}