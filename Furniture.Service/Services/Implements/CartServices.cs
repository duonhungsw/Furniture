namespace Furniture.Service;

public class CartServices(
    ICartRepository _cartRepository,
    IAccountRepository _accountRepository,
    IProductRepository _productRepository,
    ICartItemRepository _cartItemRepository) : ICartServices
{
    public async Task<List<CartItemDto>?> GetCartsAsync(Guid accountId)
    {
        var list = await _cartRepository.GetCartProductsAsync(accountId);
        return list?.OrderByDescending(x => x.CreatedAt).ToList() ?? new List<CartItemDto>();
    }
    public async Task<List<CartItem>?> GetCartItemsByAccountIdAsync(Guid accountId)
    {
        var list = await _cartItemRepository.GetCartItemByUserIdAsync(accountId);
        return list;

    }
         
    public async Task<bool> DeleteCartItemAsync(Guid cartItemID)
    {
        var cartItem = await _cartItemRepository.GetByIdAsync(cartItemID);
        if (cartItem != null)
        {
            _cartItemRepository.Delete(cartItem);
            var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
            product!.QuantityInStock += cartItem.Quantity;
            await _cartItemRepository.SaveChangesAsync();
            return true;
        }
        return false;
    }
    public async Task<bool> UpdateCartItemByQuantityAsync(Guid cartItemId, int quantity)
    {

        var cartItem = await _cartItemRepository.GetByIdAsync(cartItemId);
        if (cartItem != null)
        {
            cartItem.Quantity = quantity;
            cartItem.TotalMoney = quantity * cartItem.Price;
            _cartItemRepository.Update(cartItem);
            await _cartItemRepository.SaveChangesAsync();
            return true;
        }
        return false;
    }
    public async Task<bool> UpdateCartItemByStatusAsync(Guid cartItemId)
    {
        var cartItem = await _cartItemRepository.GetByIdAsync(cartItemId);
        if (cartItem != null)
        {
            if (cartItem.Status)
            {
                cartItem.Status = false;
                _cartItemRepository.Update(cartItem);
                await _cartItemRepository.SaveChangesAsync();
            }
            else if (!cartItem.Status)
            {
                cartItem.Status = true;
                _cartItemRepository.Update(cartItem);
                await _cartItemRepository.SaveChangesAsync();
            }
            return true;
        }
        return false;
    }
    public async Task<bool> AddCartItemAsync(CartAddDto model, Guid accountId)
    {
        Guid productId = model.ProductId;
        int quantity = model.Quantity;

        var account = await _accountRepository.GetByIdAsync(accountId);
        if (account != null)
        {
            var cartByAccountId = await _cartRepository.GetCartByAccountIdAsync(account.Id);
            var productByProductId = await _productRepository.GetByIdAsync(productId);
            var cartItembByAccountIdAndProductId = await _cartItemRepository.GetCartItemByCartIdAndProductIdAsync(cartByAccountId.Id, productId);
            if (await _cartItemRepository.CheckCartItemByProductIdAsync(cartByAccountId!, productByProductId!.Id))
            {
                if(cartItembByAccountIdAndProductId.Quantity + model.Quantity >= productByProductId.QuantityInStock)
                {
                    cartItembByAccountIdAndProductId.Quantity = productByProductId.QuantityInStock;
                    _cartItemRepository.Update(cartItembByAccountIdAndProductId);
                    await _cartItemRepository.SaveChangesAsync();
                    return true;
                }
                var cartItem = await _cartItemRepository.GetCartItemByCartIdAndProductIdAsync(cartByAccountId!.Id, productId);
                await _cartItemRepository.AddCartItemIsContainAsync(cartItem!, quantity);
                cartItem!.TotalMoney = cartItem.Price * cartItem.Quantity;
                _cartItemRepository.Update(cartItem);
                await _cartItemRepository.SaveChangesAsync();
                return true;
            }
            else
            {
                var cartItem = new CartItem();
                cartItem.ProductId = productId;
                cartItem.CartId = cartByAccountId!.Id;
                cartItem.Quantity = quantity;
                cartItem.Status = false;
                cartItem.Price = productByProductId.Price;
                cartItem.TotalMoney = quantity * productByProductId.Price;
                await _cartItemRepository.AddCartItemAsync(cartItem);
                return true;
            }
        }
        return false;
    }
    public async Task<bool> IsCartItemExistsAsync(Guid productId, Guid accountId)
    {
        return await _cartItemRepository.IsCartItemExistsAsync(productId, accountId);
    }
    public async Task<bool> IsContainProduct(Guid accountId ,Guid productId)
    {
        var cart = await _cartRepository.GetCartByAccountIdAsync(accountId);
        var cartItem  = await _cartItemRepository.GetCartItemByCartIdAndProductIdAsync(cart.Id, productId);
        if(cartItem != null)
        {
            return true;
        }
        return false;

    }

}