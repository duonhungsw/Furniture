namespace Furniture.Service;

public interface ISmsService
{
	Task<bool> SendSmsAsync(SmsRequest request);
}
