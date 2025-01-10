using Microsoft.AspNetCore.Mvc;

namespace Furniture.Web.Controllers;

public class OrderController : Controller
{
    public IActionResult Checkout()
    {
        return View();
    }
}
