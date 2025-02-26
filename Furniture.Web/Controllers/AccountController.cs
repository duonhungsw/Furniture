using Furniture.Web.Services;

namespace Furniture.Web.Controllers;

public class Account(IAccountApi accountApi) : Controller
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
			var token = await accountApi.LoginAsync(model);
			if (token == null || token.AccessToken == null || token.RefreshToken == null) return RedirectToAction("Login");

			var cookieOptions = new CookieOptions
			{
				HttpOnly = false,
				Secure = false,
				SameSite = SameSiteMode.Strict,
			};
			HttpContext?.Response.Cookies.Append("AccessToken", token.AccessToken, cookieOptions);
			HttpContext?.Response.Cookies.Append("RefreshToken", token.RefreshToken, cookieOptions);
			TempData["SuccessMessage"] = "Đăng nhập thành công!";

			return RedirectToAction("Index", "Home");
		}
		catch (ApiException ex)
		{
			return RedirectToAction("Login");
		}
	}
	[HttpPost]
	public async Task<IActionResult> SignUp(SignupDTOs model)
	{
		var result = await accountApi.RegisterAsync(model);
		//if (!result!.IsSuccessStatusCode || result.Content == null)
		//{
		//	return View();
		//}
		return RedirectToAction("Login");
	}
	[HttpGet]
	public ActionResult ForgotPassword()
	{
		return View();
	}
	[HttpPost]
	public async Task<IActionResult> ForgotPassword(string Email)
	{
		try
		{
			await accountApi.ForgotPassword(Email);

			TempData["Success"] = "Reset password email has been sent successfully. Please check your email.";
		}
		catch (Exception)
		{
			TempData["Error"] = "Failed to send reset password email. Please try again.";
		}

		return RedirectToAction("ForgotPassword");
	}

	public IActionResult UpdatePassword()
	{
		return View();
	}
	[HttpPost]
	public async Task<IActionResult> UpdatePassword(ForgotPassDTOs forgotPassDTOs)
	{
		if (forgotPassDTOs.Password != forgotPassDTOs.ConfirmPassword)
		{
			ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
			return View(forgotPassDTOs);
		}

		var result = await accountApi.UpdatePassword(forgotPassDTOs);

		if (result)
		{
			TempData["SuccessMessage"] = "Password updated successfully!";
			return RedirectToAction("Login", "Account");
		}
		else
		{
			ModelState.AddModelError("", "Failed to update password. Please try again.");
			return View(forgotPassDTOs);
		}
	}
	public IActionResult ViewProfile()
	{
		return View();
	}
	public IActionResult Logout()
	{
		//accountApi.Logout();
		HttpContext?.Response.Cookies.Delete("AccessToken");
		HttpContext?.Response.Cookies.Delete("RefreshToken");
		return RedirectToAction("Login");
	}
}
