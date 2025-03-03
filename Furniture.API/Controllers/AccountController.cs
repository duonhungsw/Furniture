using Microsoft.AspNetCore.Mvc;

namespace Furniture.API.Controllers;
[Route("accounts")]
public class AccountController(IAccountServices _accountServices, ITokenService tokenService,
								MailService sendMail) : BaseApiController
{
	[HttpGet]
	public async Task<ActionResult<PagedResult<AccountDto>>> GetAccounts([FromQuery] QueryInfo queryInfo)
	{
		var accounts = await _accountServices.GetAccountsAsync();
		return CreatePagedResult(accounts, queryInfo);
	}
	[HttpPost("login")]
	public async Task<ActionResult<TokenDto>> Login([FromBody] SignInDTOs model)
	{
		var result = await _accountServices.LoginAsync(model);
		return Ok(result);
	}
	[HttpPost("register")]
	public async Task<IActionResult> Register([FromBody] SignupDTOs signupDTOs)
	{
		var result = await _accountServices.RegisterAsync(signupDTOs);
		return Ok($"Success: {result}");
	}
	[HttpPost("logout")]
	public IActionResult Logout()
	{
		tokenService.RemoveTokenInCookie();
		return NoContent();
	}
	[HttpGet("user_info")]
	public async Task<ActionResult<Account>> GetUserInfo()
	{
		var account = await tokenService.Authenticate();

		if (account == null)
		{
			return NotFound(new { message = "User not found" });
		}

		return Ok(account);
	}
	[HttpGet("get-by-id/{id}")]
	public async Task<ActionResult<AccountDto>> GetCustomerById(Guid id)
	{
		var result = await _accountServices.GetAccountByIdAsync(id);
		return Ok(result);
	}

	[HttpPost("forgot-password/email")]
	public async Task<ActionResult> ForgotPassword([FromQuery] string email)
	{
		var user = await _accountServices.GetAccountByEmailAsync(email);
		if (user == null) return BadRequest("Email does not exist");

		HttpContext.Session.SetString("UserEmail", email);

		var resetPasswordUrl = "https://localhost:7070/Account/UpdatePassword";

		MailContent mail = new MailContent
		{
			To = user.Email,
			Subject = "Reset Password - Furniture Shop",
			Body = "<h3>Click the link to reset your password:</h3>\n" +
				   $"<a href='{resetPasswordUrl}'>Reset Password</a>\n" +
				   $"<p>If you didn't request this, please ignore this email.</p>"
		};

		bool send = await sendMail.SendMail(mail);
		if (send)
		{
			return Ok("Reset password email has been sent successfully.");
		}
		else
		{
			return BadRequest("Failed to send email.");
		}
	}

	[HttpPost("update-password")]
	public async Task<bool> ResetPassword([FromBody] ForgotPassDTOs forgotPasswordModel)
	{
		var email = HttpContext.Session.GetString("UserEmail");
		if (email == null) return false;

		var result = await _accountServices.ResetPasswordAsync(email, forgotPasswordModel);
		if (!result)
			return false;

		return true;
	}
	[HttpPut("profile")]
	public async Task<bool> UpdateProfile([FromForm] UpdateAccountDto model)
	{
		return await _accountServices.UpdateAsync(model);
	}

}
