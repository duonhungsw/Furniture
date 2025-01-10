using Furniture.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Furniture.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
