using Furniture.Web.Models;
using Furniture.Web.Services;

namespace Furniture.Web.Controllers;

public class HomeController(IAccountService accountService) : Controller
{
    public async Task<ActionResult<AccountModel>> Index()
    {
        var result = await accountService.GetUserInfo();
        return View(result);
    }
}
