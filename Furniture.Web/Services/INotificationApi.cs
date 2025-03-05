namespace Furniture.Web.Services;

public interface INotificationApi
{
	[Post("/notifications/sms")]
	Task<bool> SendSms(SmsRequest request);
	[Post("/notifications/email")]
	Task<bool> SendEmail(MailContent request);
}
