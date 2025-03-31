using System.Net;
using Microsoft.AspNetCore.Authorization;
namespace Furniture.Web.Controllers;
//[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = "CookieAuth", Roles = "Customer")]
[Microsoft.AspNetCore.Authorization.Authorize(Roles = "Customer")]
public class CartController(ICartApi cartApi, IAccountApi accountApi) : Controller
{
    [HttpGet]
    public async Task<IActionResult> ShoppingCart()
    {
        var account = await accountApi.GetUserInfoAsync();
            var response = await cartApi.GetCarts(account!.Content!.Id);
        var item = 0;
        int itemsNumber = HttpContext.Session.GetInt32("itemsNumber") ?? 0;
        foreach(var items in response.Content)
        {
            item = item + 1;
        }
        HttpContext.Session.SetInt32("itemsNumber", item);
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
            bool result = await cartApi.AddCartItem(model, account!.Content!.Id);
            return result;
        }
        else
        {
            RedirectToAction("Account", "Login");
        }
        return false;
    }
    [HttpGet]
    public async Task<IActionResult> CheckProduct([FromQuery] Guid productId)
    {
        try
        {
            var account = await accountApi.GetUserInfoAsync();
            if (account?.Content == null)
            {
                return BadRequest("User account not found.");
            }

            bool productExists = await cartApi.CheckProduct(account.Content.Id, productId);
            return Ok(productExists);
        }
        catch (Exception ex)
        {
            // Log the exception if necessary
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}