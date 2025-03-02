using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(System.TimeProvider.System);
builder.Services.AddAuthorization();
builder.Services.AddDistributedMemoryCache();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("Database"))
);
//string? connectionString = builder.Configuration.GetValue<string>("AzureBlobStorage:ConnectionString");

//if (string.IsNullOrEmpty(connectionString))
//{
//	throw new InvalidOperationException("Azure Blob Storage connection string is missing or empty.");
//}

//builder.Services.AddSingleton<IFileStorageService>(provider =>
//	new FileStorageService(connectionString));


builder.Services.AddSingleton<IFileStorageService>(provider =>
	new FileStorageService(builder.Configuration.GetSection("AzureBlobStorage:ConnectionString").Value!));

builder.Services.AddSingleton(nameof(ApplicationDbContext));

//add inject Repositories and Services

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();

builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IAccountServices, AccountServices>();
builder.Services.AddScoped<IProductServices, ProductServices>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartServices, CartServices>();
//builder.Services.AddScoped<IFileStorageService, FileStorageService>();

builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

builder.Services.AddCors(options =>
{
	options.AddPolicy(name: MyAllowSpecificOrigins,
		builder =>
		{
			builder.WithOrigins("https://localhost:7000")
			.AllowCredentials() 
			.AllowAnyMethod()
			.AllowAnyHeader()
			.SetIsOriginAllowedToAllowWildcardSubdomains().AllowCredentials();
		});
});
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(5);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
	options.Cookie.SameSite = SameSiteMode.Lax;
});

// Configure email service
var mailsettings = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailsettings);
builder.Services.AddTransient<SendMailService>();

builder.Services.AddFluentValidationAutoValidation()
				.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);
app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

try
{
	using var scope = app.Services.CreateScope();
	var services = scope.ServiceProvider;
	var context = services.GetRequiredService<ApplicationDbContext>();
	await context.Database.MigrateAsync();
	await StoreContextSeed.SeedAsync(context);

}
catch (Exception ex)
{
	Console.WriteLine(ex.ToString());
	throw;
}

app.Run();

