namespace Furniture.Service;

public class CartServices(
    ICartRepository _cartRepository,
    IAccountRepository _accountRepository,
    IProductRepository _productRepository,
    ICartItemRepository cartItemRepository) : ICartServices
{
    public async Task<List<CartItemDto>?> GetCartsAsync(Guid accountId)
    {
        var list = await _cartRepository.GetCartProductsAsync(accountId);
        return list?.OrderByDescending(x => x.CreatedAt).ToList() ?? new List<CartItemDto>();
    }
         
    public async Task<bool> DeleteCartItemAsync(Guid cartItemID)
    {
        var cartItem = await cartItemRepository.GetByIdAsync(cartItemID);
        if (cartItem != null)
        {
            cartItemRepository.Delete(cartItem);
            var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
            product.QuantityInStock += cartItem.Quantity;
            await cartItemRepository.SaveChangesAsync();
            return true;
        }
        return false;
    }
    public async Task<bool> UpdateCartItemByQuantityAsync(Guid cartItemId, int quantity)
    {

        var cartItem = await cartItemRepository.GetByIdAsync(cartItemId);
        if (cartItem != null)
        {
            cartItem.Quantity = quantity;
            cartItem.TotalMoney = quantity * cartItem.Price;
            cartItemRepository.Update(cartItem);
            await cartItemRepository.SaveChangesAsync();
            return true;
        }
        return false;
    }
    public async Task<bool> UpdateCartItemByStatusAsync(Guid cartItemId)
    {
        var cartItem = await cartItemRepository.GetByIdAsync(cartItemId);
        if (cartItem != null)
        {
            if (cartItem.Status)
            {
                cartItem.Status = false;
                cartItemRepository.Update(cartItem);
                await cartItemRepository.SaveChangesAsync();
            }
            else if (!cartItem.Status)
            {
                cartItem.Status = true;
                cartItemRepository.Update(cartItem);
                await cartItemRepository.SaveChangesAsync();
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
            if (await cartItemRepository.CheckCartItemByProductIdAsync(cartByAccountId!, productByProductId!.Id))
            {
                var cartItem = await cartItemRepository.GetCartItemByCartIdAndProductIdAsync(cartByAccountId!.Id, productId);
                await cartItemRepository.AddCartItemIsContainAsync(cartItem!, quantity);
                cartItem!.TotalMoney = cartItem.Price * cartItem.Quantity;
                cartItemRepository.Update(cartItem);
                await cartItemRepository.SaveChangesAsync();
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
                await cartItemRepository.AddCartItemAsync(cartItem);
                return true;
            }
        }
        return false;
    }


}