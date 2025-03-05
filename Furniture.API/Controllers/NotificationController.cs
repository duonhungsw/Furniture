namespace Furniture.API.Controllers;

[Route("notifications")]

public class NotificationController(ISmsService _smsService, MailService _mailService) : BaseApiController
{
	[HttpPost("sms")]
	public async Task<bool> SendSms([FromBody] SmsRequest request)
	{
		return await _smsService.SendSmsAsync(request);
	}
	[HttpPost("email")]
	public async Task<bool> SendEmail([FromBody] MailContent mailContent)
	{
		return await _mailService.SendMail(mailContent);
	}
}
