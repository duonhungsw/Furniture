namespace Furniture.Web.Controllers;

[Route("Notification")]
public class NotificationController(INotificationApi _api) : Controller
{
	[HttpPost("SendSms")]
	public async Task<IActionResult> SendSms([FromBody] SmsRequest request)
	{
		var otp = new Random().Next(100000, 999999).ToString();

		request.Text = $"Your verification code is: {otp}";

		var isSent = await _api.SendSms(request);
		if (isSent)
		{
			HttpContext.Session.SetString("OtpCode", otp);
			HttpContext.Session.SetString("OtpExpiry", DateTime.UtcNow.AddSeconds(60).ToString());
			//HttpContext.Session.Remove("OtpCode");
			//HttpContext.Session.Remove("OtpExpiry");
			return Ok(new { success = true});
		}
		else
		{
			return StatusCode(500, new { success = false });
		}
	}
}
