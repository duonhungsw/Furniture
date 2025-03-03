using RestSharp;

namespace Furniture.Service;

public class SmsService : ISmsService
{
	private readonly string _apiKey;
	public SmsService(IConfiguration configuration)
	{
		_apiKey = configuration["SmsSettings:ApiKey"]!;
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

		var body = new
		{
			messages = new[]
			{
				new
				{
					destinations = new[] { new { to = "84869098413" } },
					from = smsRequest.From,
					text = smsRequest.Text
				}
			}
		};

		request.AddJsonBody(body);
		RestResponse response = await client.ExecuteAsync(request);

		return !response.IsSuccessful ? false : true;
	}
}
