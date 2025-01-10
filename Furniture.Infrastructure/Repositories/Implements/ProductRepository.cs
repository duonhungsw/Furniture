namespace Furniture.Infrastructure.Repositories.Implements;


public class ProductRepository : GenericRepository<Account>, IProductRepository
{
	public ProductRepository(ApplicationDbContext context) : base(context)
	{
	}
}