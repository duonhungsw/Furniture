using Furniture.Web.Services;

namespace Furniture.Web.Controllers;

public class Account(IAccountService accountService) : Controller
{
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(SignInDTOs model)
    {
        try
        {
            var token =  await accountService.LoginAsync(model);
            
            TempData["SuccessMessage"] = "Đăng nhập thành công!";

            return RedirectToAction("Index","Home");
        }
        catch (ApiException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public IActionResult Index()
    {
        return View();
    }
}
