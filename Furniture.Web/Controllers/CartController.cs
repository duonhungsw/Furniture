using Microsoft.AspNetCore.Mvc;

namespace Furniture.Web.Controllers;

public class CartController : Controller
{
    [HttpGet]
    public IActionResult ShoppingCart()
    {
        return View();
    }
}
