
using Furniture.Core.Dtos.Cart;

namespace Furniture.Service.Services.Implements;

public class CartServices(ICartRepository cartRepository,IProductRepository productRepository ,ITokenService tokenService, IMapper mapper,ICartItemRepository cartItemRepository) : ICartServices
{
    public async Task<List<CartItemDto>?> GetCartsAsync()
    {

        var account = await tokenService.Authenticate();
        if (account != null)
        {
            return await cartRepository.GetCartProductsAsync(account.Id);
        }
        return null;
    }
    public async Task<bool> DeleteCartItem(Guid cartItemID)
    {
        var account = await tokenService.Authenticate();
        if (account != null)
        {
            var checkDelelte = await cartRepository.DeleteCartItem(account.Id, cartItemID);
            if (checkDelelte)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
    public async Task<bool> UpdateCartItemByQuantity(Guid cartItemId, int quantity)
    {
        var account = await tokenService.Authenticate();
        if (account != null)
        {
            var checkUpdate = await cartRepository.UpdateCartItemByQuantity(account.Id, cartItemId, quantity);
            if (checkUpdate)
            {
                return true;
            }
            else
            {
                return false;
            }


        }
        return false;
    }
    public async Task<bool> UpdateCartItemByStatus(Guid cartItemId)
    {
        var account = await tokenService.Authenticate();
        if (account != null)
        {
            var checkUpdate = await cartRepository.UpdateCartItemByStatus(account.Id,cartItemId);
            if (checkUpdate)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
    public async Task<bool> AddCartItem(CartAddDto model)
    {
        Guid productId = model.ProductId;
        int quantity = model.Quantity;
        var account = await tokenService.Authenticate();
        if (account != null)
        {
            var cartByAccountId = await cartRepository.GetCartByAccountId(account.Id);
            var productByProductId = await productRepository.FindByIdAsync(productId);
            if (await cartItemRepository.CheckCartItemByProductId(cartByAccountId, productByProductId.Id))
            {
                var cartItem = await cartItemRepository.GetCartItemByCartIdAndProductId(cartByAccountId.Id, productId);
                await cartItemRepository.AddCartItemIsContain(cartItem, quantity);
            }
            else
            {
                var cartItem = new CartItem();
                cartItem.ProductId = productId;
                cartItem.CartId = cartByAccountId.Id;
                cartItem.Quantity = quantity;
                cartItem.Status = false;
                cartItem.Price = productByProductId.Price;
                await cartItemRepository.AddCartItem(cartItem);
                return true;
            }
        }
        return false;
    }
    public async Task<Cart> GetCartByAccountId()
    {
        var account = await tokenService.Authenticate();
        if(account != null)
        {
            var cart = await cartRepository.GetCartByAccountId(account.Id);
            return cart;
        }
        return null;
    }
}