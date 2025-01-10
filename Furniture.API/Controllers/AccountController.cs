using Furniture.Core.Dtos.Account;
using Furniture.Core.DTOs;
using Furniture.Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Furniture.API.Controllers;

public class AccountController(IAccountServices accountServices, ITokenService tokenService,
								SendMailService sendMail) : BaseApiController
{
	[HttpPost("/Login")]
	public async Task<ActionResult<TokenDto>> Login([FromBody] SignInDTOs model)
	{
		var result = await accountServices.LoginAsync(model);
		return Ok(result);
	}
	[HttpPost("/Register")]
	public async Task<IActionResult> Register([FromBody] SignupDTOs signupDTOs)
	{
		var result = await accountServices.RegisterAsync(signupDTOs);
		return Ok($"Success: {result}");
	}
	[HttpPost("/Logout")]
	public async Task<IActionResult> Logout()
	{
		tokenService.RemoveTokenInCookie();
		return NoContent();
	}
	[HttpGet("/user_info")]
	public async Task<ActionResult<Account>> GetUserInfo()
	{
		var account = await tokenService.Authenticate();
		return Ok(account);
	}

	//[HttpPost("/forgot-password")]
	//public async Task<ActionResult> ForgotPassword([FromQuery] string email)
	//{
	//	var user = await signInManager.UserManager.FindByEmailAsync(email);
	//	if (user == null) return BadRequest("Email does not exist");

	//	var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
	//	Console.WriteLine(resetToken);
	//	var resetPasswordUrl = Url.Action(
	//		"ResetPassword",
	//		"Account",
	//		new { token = resetToken, email = email },
	//		Request.Scheme);

	//	MailContent mail = new MailContent
	//	{
	//		To = user.Email,
	//		Subject = "Reset Password - Furniture Shop",
	//		Body = "<h3>Click the link to reset your password:</h3>\n" +
	//			   $"<a href='{resetPasswordUrl}'>Reset Password</a>\n" +
	//			   $"<p>If you didn't request this, please ignore this email.</p>"
	//	};

	//	bool send = await sendMail.SendMail(mail);
	//	if (send)
	//	{
	//		return Ok("Reset password email has been sent successfully.");
	//	}
	//	else
	//	{
	//		return BadRequest("Failed to send email.");
	//	}
	//}

	//[HttpPost("/reset-password")]
	//public async Task<IActionResult> ResetPassword([FromForm] ForgotPassDTOs forgotPasswordModel)
	//{
	//	if (!ModelState.IsValid) return BadRequest("Invalid request");

	//	var user = await userManager.FindByEmailAsync(forgotPasswordModel.email!);
	//	if (user == null) return BadRequest("Email does not exist");

	//	var result = await userManager.ResetPasswordAsync(user, forgotPasswordModel.token!, forgotPasswordModel.newPassword!);
	//	if (result.Succeeded)
	//	{
	//		return Ok("Password has been reset successfully.");
	//	}

	//	foreach (var error in result.Errors)
	//	{
	//		ModelState.AddModelError(string.Empty, error.Description);
	//	}
	//	return ValidationProblem(ModelState);
	//}
}
