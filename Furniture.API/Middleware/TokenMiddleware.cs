using Microsoft.AspNetCore.Authorization;

public class TokenMiddleware
{
	private readonly RequestDelegate _next;
	private readonly IServiceScopeFactory _serviceScopeFactory;

	public TokenMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
	{
		_next = next;
		_serviceScopeFactory = serviceScopeFactory;
	}

	public async Task Invoke(HttpContext context)
	{
		var endpoint = context.GetEndpoint();
		var hasAuthorize = endpoint?.Metadata?.GetMetadata<AuthorizeAttribute>() != null;

		if (hasAuthorize)
		{
			using (var scope = _serviceScopeFactory.CreateScope())
			{
				var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
				var account = await tokenService.GetTokenAsync();

				if (account != null)
				{
					context.Items["User"] = account;
				}
				else
				{
					tokenService.RemoveTokenInCookie();
					context.Response.StatusCode = StatusCodes.Status401Unauthorized;
					return;
				}
			}
		}

		await _next(context);
	}
}
