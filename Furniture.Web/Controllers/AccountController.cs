using Microsoft.AspNetCore.Mvc;

namespace Furniture.Web.Controllers;

public class Account : Controller
{
	public IActionResult Login()
	{
		return View();
	}
	public IActionResult Index()
	{
		return View();
	}
}
