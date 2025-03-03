namespace Furniture.Infrastructure;

public class ApplicationDbContext : DbContext
{
	public ApplicationDbContext() { }
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
	{
	}
	public virtual DbSet<Account> Accounts { get; set; }
	public virtual DbSet<Product> Products { get; set; }
	public virtual DbSet<Cart> Carts { get; set; }
	public virtual DbSet<CartItem> CartItems { get; set; }
	public virtual DbSet<Order> Orders { get; set; }
	public virtual DbSet<OrderItem> OrderItems { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		var builder = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
		IConfiguration configuration = builder.Build();
		optionsBuilder.UseSqlServer(configuration.GetConnectionString("Database"));
	}
}
