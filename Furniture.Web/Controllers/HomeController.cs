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

		return View(response.Content);
	}
}
