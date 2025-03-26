namespace Furniture.Web.Controllers;

public class HomeController(IAccountApi accountService) : Controller
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

		return View(response.Content);
	}
}
