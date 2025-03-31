using Furniture.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Furniture.Web.Controllers;

public class HomeController(IAccountApi accountService, IProductApi _productApi) : Controller
{
	public async Task<IActionResult> Index()
	{
		var response = await accountService.GetUserInfoAsync();
        if (!response!.IsSuccessStatusCode || response.Content == null)
		{

            QueryInfo _queryInfo = new QueryInfo()
            {
                PageIndex = 1,
                PageSize = 3
            };
            var _result = await _productApi.GetProductsAsync(_queryInfo);
            ViewBag.products = _result.Items;
            return View();
		}
        var claims = new List<Claim>
                {
        new Claim(ClaimTypes.NameIdentifier, response.Content.Id.ToString()),
        new Claim(ClaimTypes.Name, response.Content.Name),
        new Claim(ClaimTypes.Role, response.Content.RoleName!)
                };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity), authProperties);
        if (response.Content.RoleName == AppRoles.Admin.ToString())
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
