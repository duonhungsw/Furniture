using Microsoft.AspNetCore.Mvc;

namespace Furniture.Web.Controllers;

public class CartController : Controller
{
    [HttpGet]
    public async Task<IActionResult> ShoppingCart()
    {
        return RedirectToAction("ShoppingCart", "Cart");
    }
}
