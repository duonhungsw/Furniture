using Furniture.Web.Services;

namespace Furniture.Web.Controllers;

public class CartController(ICartApi cartApi, IAccountApi accountApi) : Controller
{
    [HttpGet]
    public async Task<IActionResult> ShoppingCart()
    {
        var account = await accountApi.GetUserInfoAsync();
            var response = await cartApi.GetCarts(account!.Content!.Id);

        if (!response.IsSuccessStatusCode || response.Content == null)
        {
            return View(new List<CartItemDto>()); // Nếu lỗi, trả về danh sách rỗng
        }

        return View(response.Content); // Chỉ truyền danh sách CartItemDto vào View
    }
    [HttpPatch]
    public async Task<bool> UpdateCartItemByQuantity([FromBody] CartUpdateQuantityDto model)
    {
        var account = await accountApi.GetUserInfoAsync();
        if (account != null)
        {
            var response = await cartApi.UpdateCartItemByQuantity(model);
            return response;
        }
        return false;
    }
    [HttpPatch]
    public async Task<bool> UpdateCartItemByStatus([FromRoute] CartItemDto model)
    {
        var account = await accountApi.GetUserInfoAsync();
        if (account != null)
        {
            var result = await cartApi.UpdateCartItemByStatus(model.Id);
            return result;
        }
        return false;
    }
    [HttpDelete]
    public async Task<bool> DeleteCartItem([FromRoute] CartItemDto model)
    {
        var account = await accountApi.GetUserInfoAsync();
        if (account != null)
        {
            bool result = await cartApi.DeleteCartItem(model.Id);
            int itemsNumber = HttpContext.Session.GetInt32("itemsNumber") ?? 0;
            HttpContext.Session.SetInt32("itemsNumber", itemsNumber - 1);
            return result;
        }
        return false;
    }
    [HttpPost]
    public async Task<bool> AddCartItem([FromForm] CartAddDto model)
    {
        var account = await accountApi.GetUserInfoAsync();
        if (account != null)
        {
            bool result = await cartApi.AddCartItem(model,account!.Content!.Id);
            int itemsNumber = HttpContext.Session.GetInt32("itemsNumber") ?? 0;
            HttpContext.Session.SetInt32("itemsNumber", itemsNumber + 1); // Lưu Session
            return result;
        }
        return false;
    }


}