using Furniture.Common.Errors;
using System.Net;
using System.Text.Json;

namespace Furniture.API.Middleware;

public class ExceptionMiddleware
{
	private readonly RequestDelegate _next;
	private readonly IHostEnvironment _env;

	public ExceptionMiddleware(IHostEnvironment env, RequestDelegate next)
	{
		_next = next;
		_env = env;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception ex)
		{
			await HandleExceptionAsync(context, ex);
		}
	}

	private async Task HandleExceptionAsync(HttpContext context, Exception ex)
	{
		context.Response.ContentType = "application/json";

		int statusCode;
		string message;
		string details;

		if (ex is CustomException customException)
		{
			statusCode = customException.StatusCode;
			message = customException.Message;
			details = _env.IsDevelopment() ? ex.StackTrace! : "An error occurred";
		}
		else
		{
			statusCode = (int)HttpStatusCode.InternalServerError;
			message = "Internal Server Error";
			details = _env.IsDevelopment() ? ex.StackTrace! : "An unexpected error occurred";
		}

		context.Response.StatusCode = statusCode;

		var response = new
		{
			status = statusCode,
			message,
			details
		};

		var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
		var json = JsonSerializer.Serialize(response, options);

		await context.Response.WriteAsync(json);
	}
}