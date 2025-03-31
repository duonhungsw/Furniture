using Furniture.Web.Services.VnPay;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.Net;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});
// Đăng ký Refit Client
builder.Services.AddRefitClient<IAccountApi>()
	.ConfigureHttpClient((serviceProvider, httpClient) =>
	{
		var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
		var token = httpContextAccessor.HttpContext?.Request.Cookies["AccessToken"];

		if (!string.IsNullOrEmpty(token))
		{
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		}

		httpClient.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
	});
builder.Services.AddRefitClient<INotificationApi>()
	.ConfigureHttpClient(c =>
	{
		c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
	});
builder.Services.AddRefitClient<IProductApi>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
    });
builder.Services.AddRefitClient<IOrderApi>()
	.ConfigureHttpClient(c =>
	{
		c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
	});
builder.Services.AddRefitClient<ICartApi>()
    .ConfigureHttpClient(c =>
    {	
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
    });
builder.Services.AddRefitClient<IStatusApi>()
	.ConfigureHttpClient(c =>
	{
		c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
	});


builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian lưu session
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});
//builder.Services.AddAuthentication("CookieAuth")
//	.AddCookie("CookieAuth", config =>
//	{
//		config.LoginPath = "/Account/Login";
//		config.AccessDeniedPath = "/Account/Login";
//	});
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        //options.LoginPath = "/Home/Login";
//        options.AccessDeniedPath = "/Account/Login";
//        options.Cookie.HttpOnly = false;
//        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
//    });

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Use Google scheme here
})
.AddCookie(options =>
{
    //options.LoginPath = "/Home/Login";
    options.AccessDeniedPath = "/Account/Login";
    options.Cookie.HttpOnly = false;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
})
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
	options.ClientId = builder.Configuration.GetSection("Authentication:Google:ClientId").Value!;
	options.ClientSecret = builder.Configuration.GetSection("Authentication:Google:ClientSecret").Value!;
});

builder.Services.AddScoped<IVnPayService, VnPayService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}
app.UseSession();

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
