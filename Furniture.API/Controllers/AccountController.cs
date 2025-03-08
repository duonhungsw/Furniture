using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Furniture.API.Controllers;

[Route("accounts")]
public class AccountController(IAccountServices _service, ITokenService _tokenService,
								MailService sendMail) : BaseApiController
{
	//var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
	[HttpGet]
	public async Task<ActionResult<PagedResult<AccountDto>>> GetAccounts([FromQuery] QueryInfo queryInfo)
	{
		var accounts = await _service.GetAccountsAsync();
		return CreatePagedResult(accounts, queryInfo);
	}
	[HttpPost("login")]
	public async Task<ActionResult<TokenResponse>> Login([FromBody] SignInDTOs model)
	{
		var result = await _service.LoginAsync(model);
		return Ok(result);
	}
	[HttpPost("register")]
	public async Task<IActionResult> Register([FromBody] SignupDTOs signupDTOs)
	{
		var result = await _service.RegisterAsync(signupDTOs);
		return Ok($"Success: {result}");
	}
	[HttpPost("logout")]
	public IActionResult Logout()
	{
		_tokenService.RemoveTokenInCookie();
		return NoContent();
	}

	[HttpGet("user_info")]
	public ActionResult<Account> GetUserInfo()
	{
		var account = new Account
		{
			Id = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
			Name = User.FindFirstValue(ClaimTypes.NameIdentifier)!,
			Email = User.FindFirstValue(ClaimTypes.Name)!,
			RoleName = User.FindFirstValue(ClaimTypes.Role)!
		};
		return Ok(account);
	}
	[HttpGet("get-by-id/{id}")]
	public async Task<ActionResult<AccountDto>> GetCustomerById(Guid id)
	{
		var result = await _service.GetAccountByIdAsync(id);
		return Ok(result);
	}

	[HttpPost("forgot-password/email")]
	public async Task<ActionResult> ForgotPassword([FromQuery] string email)
	{
		var user = await _service.GetAccountByEmailAsync(email);
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

		var result = await _service.ResetPasswordAsync(email, forgotPasswordModel);
		if (!result)
			return false;

		return true;
	}
	[HttpPut("profile")]
	public async Task<bool> UpdateProfile([FromForm] UpdateAccountDto model)
	{
		return await _service.UpdateAsync(model);
	}
	[HttpPatch("changePhoneNumber")]
	public async Task<bool> UpdatePhoneNumber([FromBody] ChangePhoneNumberDto model)
	{
		return await _service.UpdatePhoneNumberAsync(model.Id, model.Phone!);
	}
<<<<<<< Updated upstream
	[HttpPost("action")]
	public async Task<bool> HandleAccountAction([FromBody] AccountActionDto request)
	{
		request.Id = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
		return await _service.HandleAccountAction(request);
=======
    [HttpPatch("role")]
    public async Task<bool> UpdateRole([FromBody] AccountDto model)
    {
		bool result = await _service.UpdateRoleAsync(model);
		return result;
>>>>>>> Stashed changes
	}
}
