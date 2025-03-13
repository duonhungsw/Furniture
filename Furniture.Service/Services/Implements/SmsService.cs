using RestSharp;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Furniture.Service;

public class SmsService : ISmsService
{
	private readonly string _apiKey;
	private readonly string _accountSid;
	private readonly string _authToken;
	private readonly string _myPhoneNumber;
	public SmsService(IConfiguration configuration)
	{
		_apiKey = configuration["SmsSettings:ApiKey"]!;
		_accountSid = configuration["SmsByTwilioSettings:TwilioAccountSid"]!;
		_authToken = configuration["SmsByTwilioSettings:TwilioAuthToken"]!;
		_myPhoneNumber = configuration["SmsByTwilioSettings:MyPhoneNumber"]!;
	}
	public async Task<bool> SendSmsAsync(SmsRequest smsRequest)
	{
		var options = new RestClientOptions("https://api.infobip.com")
		{
			MaxTimeout = -1,
		};
		var client = new RestClient(options);
		var request = new RestRequest("/sms/2/text/advanced", Method.Post);
		request.AddHeader("Authorization", _apiKey);
		request.AddHeader("Content-Type", "application/json");
		request.AddHeader("Accept", "application/json");
		var to = FormatPhoneNumber(smsRequest.To!);
		var body = new
		{
			messages = new[]
			{
				new
				{
					destinations = new[] { new { to = to } },
					from = "447491163443",
					text = smsRequest.Text
				}
			}
		};

		request.AddJsonBody(body);
		RestResponse response = await client.ExecuteAsync(request);

		return !response.IsSuccessful ? false : true;
	}
	private string FormatPhoneNumber(string phoneNumber)
	{
		if (phoneNumber.StartsWith("0"))
		{
			return "+84" + phoneNumber.Substring(1); // Chuyển 033... → +8433...
		}
		return phoneNumber; // Nếu đã có +84 thì giữ nguyên
	}
}
