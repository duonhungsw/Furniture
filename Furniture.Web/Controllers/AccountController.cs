namespace Furniture.Web.Controllers;


public class Account(IAccountApi _accountApi) : Controller
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
            var token = await _accountApi.LoginAsync(model);
            if (token == null || token.AccessToken == null || token.RefreshToken == null) return RedirectToAction("Login");

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            HttpContext?.Response.Cookies.Append("AccessToken", token.AccessToken, cookieOptions);
            HttpContext?.Response.Cookies.Append("RefreshToken", token.RefreshToken, cookieOptions);

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
        var result = await _accountApi.RegisterAsync(model);
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
            await _accountApi.ForgotPasswordAsync(Email);

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

        var result = await _accountApi.UpdatePasswordAsync(forgotPassDTOs);

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
    public async Task<ActionResult<AccountDto>> ViewProfile()
    {
        var response = await _accountApi.GetUserInfoAsync();

        if (response == null || !response.IsSuccessStatusCode || response.Content == null)
        {
            return RedirectToAction("Login");
        }

        var account = response.Content;
        var result = await _accountApi.GetAccountByIdAsync(account.Id);

        return View(result);
    }
    //[HttpPost]
    //public async Task<IActionResult> UpdateProfile([FromForm] UpdateAccountDto model)
    //{
    //	var result = await _services.UpdateAsync(model);
    //	if(result == true)
    //	{
    //		return RedirectToAction("ViewProfile");
    //	}
    //		return RedirectToAction("ViewProfile");
    //	//StreamPart? avatarStream = null;

    //	//if (model.Avatar != null)
    //	//{
    //	//	var memoryStream = new MemoryStream();
    //	//	await model.Avatar.CopyToAsync(memoryStream);
    //	//	memoryStream.Position = 0;

    //	//	avatarStream = new StreamPart(memoryStream, model.Avatar.FileName, model.Avatar.ContentType);
    //	//}

    //	//try
    //	//{
    //	//	var result = await _accountApi.UpdateProfileAsync(
    //	//		model.Id, model.Name, model.BirthDay, model.Phone, avatarStream);

    //	//	return RedirectToAction("ViewProfile");
    //	//}
    //	//catch (ValidationApiException ex)
    //	//{
    //	//	Console.WriteLine($"API Validation Error: {ex.Content}");
    //	//	ModelState.AddModelError(string.Empty, "Failed to update profile.");
    //	//	return View(model);
    //	//}
    //}
    [HttpPost]
    public async Task<IActionResult> UpdateProfile([FromForm] UpdateAccountDto model)
    {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:7000");

        using var content = new MultipartFormDataContent();

        // Thêm dữ liệu thông thường
        content.Add(new StringContent(model.Id.ToString()), "Id");
        if (!string.IsNullOrEmpty(model.Name))
            content.Add(new StringContent(model.Name), "Name");
        if (!string.IsNullOrEmpty(model.BirthDay))
            content.Add(new StringContent(model.BirthDay), "BirthDay");
        if (!string.IsNullOrEmpty(model.Phone))
            content.Add(new StringContent(model.Phone), "Phone");

        // Thêm file ảnh Avatar nếu có
        if (model.Avatar != null)
        {
            var memoryStream = new MemoryStream();
            await model.Avatar.CopyToAsync(memoryStream);
            memoryStream.Position = 0;  // Reset lại vị trí stream

            var fileContent = new StreamContent(memoryStream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.Avatar.ContentType);

            content.Add(fileContent, "Avatar", model.Avatar.FileName);
        }

        // Gửi request lên API
        var response = await client.PutAsync("/accounts/profile", content);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("ViewProfile");
        }

        var error = await response.Content.ReadAsStringAsync();
        return BadRequest(error);
    }

    [HttpGet]
    public IActionResult ChangePhoneNumber()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePhoneNumber([FromForm] ChangePhoneNumberDto model)
    {
        var otpSession = HttpContext.Session.GetString("OtpCode");

        //model.Session is OTP was send from form

        if (otpSession != model.session)
        {
            TempData["Message"] = "Verification code is incorrect!";
            TempData["MessageType"] = "danger";
            return RedirectToAction("ChangePhoneNumber");
        }
        var response = await _accountApi.GetUserInfoAsync();

        if (response == null || !response.IsSuccessStatusCode || response.Content == null)
        {
            return RedirectToAction("Login");
        }

        var account = response.Content;

        var accountAction = new AccountActionDto
        {
            Id = account.Id,
            NewPhoneNumber = model.Phone,
            Action = AccountAction.ChangeNumber.ToString()
        };
        var result = await _accountApi.HandleAccountAction(accountAction);

        if (result)
        {
            TempData["Message"] = "Phone number updated successfully!";
            TempData["MessageType"] = "success";

            HttpContext.Session.Remove("OtpCode");
            HttpContext.Session.Remove("OtpExpiry");
        }
        else
        {
            TempData["Message"] = "Failed to update phone number.";
            TempData["MessageType"] = "danger";
        }

        return RedirectToAction("ChangePhoneNumber");
    }
    [HttpGet]
    public IActionResult VerifyByPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> VerifyByPassword([FromForm] AccountActionDto request)
    {
        request.Action = AccountAction.VerifyByPassword.ToString();
        var response = await _accountApi.HandleAccountAction(request);
        if (!response)
        {
            TempData["ErrorMessage"] = "Password verification failed. Please try again.";
            return View();
        }
        return RedirectToAction("ChangePassword");
    }
    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> ChangePassword([FromForm] AccountActionDto request)
    {
        request.Action = AccountAction.ChangePassword.ToString();
        var response = await _accountApi.HandleAccountAction(request);
        if (!response)
        {
            TempData["ErrorMessage"] = "Password change failed. Please try again.";
            return View();
        }
        TempData["SuccessMessage"] = "Password changed successfully!";
        TempData.Keep("SuccessMessage");
        return RedirectToAction("ViewProfile");
    }
    public IActionResult Logout()
    {
        //accountApi.Logout();
        HttpContext?.Response.Cookies.Delete("AccessToken");
        HttpContext?.Response.Cookies.Delete("RefreshToken");
        return RedirectToAction("Login");
    }
    [HttpGet]
    public async Task<IActionResult> GetAccounts(int pageIndex = 1, int pageSize = 5)
    {
        var queryInfo = new QueryInfo { PageIndex = pageIndex, PageSize = pageSize };
        var pagedResult = await _accountApi.GetAccounts(queryInfo);

        return View(pagedResult.Items);
    }
    [HttpGet]
    public async Task<IActionResult> UpdateRole(Guid id)
    {
        var product = await _accountApi.GetAccountByIdAsync(id);
        return View(product);
    }
    [HttpPost]
    public async Task<IActionResult> UpdateRole([FromForm] AccountDto model)
    {
        var result = await _accountApi.UpdateRole(model);
        return RedirectToAction("GetAccounts");

    }
}


