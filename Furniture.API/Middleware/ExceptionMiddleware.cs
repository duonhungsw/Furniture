using Furniture.Common.Errors;
using System.Net;
using System.Text.Json;

namespace Furniture.API.Middleware;

public class ExceptionMiddleware(IHostEnvironment env, RequestDelegate next)
{
	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await next(context);
		}
		catch (Exception ex)
		{
			await HanldExceptionAsync(context, env, ex);
		}
	}

	private static Task HanldExceptionAsync(HttpContext context, IHostEnvironment env, Exception ex)
	{
		context.Response.ContentType = "application/json";
		context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

		var respone = env.IsDevelopment()
			? new ApiErrorsResponse(context.Response.StatusCode, ex.Message, ex.StackTrace!)
			: new ApiErrorsResponse(context.Response.StatusCode, ex.Message, "Internal server error");

		var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
		var json = JsonSerializer.Serialize(respone, options);
		return context.Response.WriteAsync(json);
	}
}
