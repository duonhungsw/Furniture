using Furniture.Web.Services;

namespace Furniture.Web.Controllers;

public class HomeController(IAccountApi accountService, IProductApi _productApi) : Controller
{
	public async Task<IActionResult> Index()
	{
		var response = await accountService.GetUserInfoAsync();

		if (!response!.IsSuccessStatusCode || response.Content == null)
		{
			return View();
		}

		if(response.Content.RoleName == AppRoles.Admin.ToString())
		{
			return RedirectToAction("Index", "Admin");
		}

		HttpContext.Session.SetObject("AccountInfo", response.Content);

		QueryInfo queryInfo = new QueryInfo()
		{
			PageIndex = 1,
			PageSize = 3
		};
		var result = await _productApi.GetProductsAsync(queryInfo);
		ViewBag.products = result.Items;

		return View(response.Content);
	}
}
