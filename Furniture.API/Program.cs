using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(System.TimeProvider.System);
builder.Services.AddAuthorization();
builder.Services.AddDistributedMemoryCache();

// Configure DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
	options.AddInterceptors(new AuditableEntityInterceptor());
});

// Configure Azure Blob Storage
builder.Services.AddSingleton<IFileStorageService>(provider =>
	new FileStorageService(builder.Configuration.GetValue<string>("AzureBlobStorage:ConnectionString") ?? throw new InvalidOperationException("AzureBlobStorage:ConnectionString is missing"))
);

// Configure JWT Authentication
var key = Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtSettings:SecretKey") ?? throw new InvalidOperationException("JwtSettings:SecretKey is missing"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidAudience = builder.Configuration.GetValue<string>("JwtSettings:ValidAudience"),
			ValidIssuer = builder.Configuration.GetValue<string>("JwtSettings:ValidIssuer"),
			IssuerSigningKey = new SymmetricSecurityKey(key)
		};
	});

// Register Repositories
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();

// Register Services
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IAccountServices, AccountServices>();
builder.Services.AddScoped<IProductServices, ProductServices>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartServices, CartServices>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISmsService, SmsService>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Configure Email Service
var mailSettings = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailSettings);
builder.Services.AddTransient<MailService>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

// Configure CORS
builder.Services.AddCors(options =>
{
	options.AddPolicy(MyAllowSpecificOrigins,
		builder =>
		{
			builder.WithOrigins("https://localhost:7000", "https://localhost:7070")
				.AllowCredentials()
				.AllowAnyMethod()
				.AllowAnyHeader();
		});
});

// Configure Session
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(5);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
	options.Cookie.SameSite = SameSiteMode.Lax;
});

// Configure Application Cookies
builder.Services.ConfigureApplicationCookie(options =>
{
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
	options.Cookie.SameSite = SameSiteMode.None;
	options.Cookie.HttpOnly = true;
});

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation()
				.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

var app = builder.Build();

// Configure the HTTP request pipeline
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
app.UseMiddleware<TokenMiddleware>();

app.MapControllers();

// Run Database Migrations and Seeding
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
	Console.WriteLine($"Error during migration: {ex.Message}");
	throw;
}

app.Run();
