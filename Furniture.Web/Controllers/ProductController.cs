using Microsoft.AspNetCore.Mvc;

namespace Furniture.Web.Controllers
{
	public class ProductController : Controller
	{
		public IActionResult ProductHome()
		{
			return View();
		}
		public IActionResult ProductDetail()
		{
			return View();
		}
	}
}
