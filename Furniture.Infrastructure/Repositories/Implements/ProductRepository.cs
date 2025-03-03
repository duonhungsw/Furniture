using Microsoft.EntityFrameworkCore;

namespace Furniture.Infrastructure;


public class ProductRepository : GenericRepository<Product>, IProductRepository
{
	public ProductRepository(ApplicationDbContext context) : base(context)
	{

	}
	public async Task<List<Product>> ListProductAsync()
	{
		return await appDbContext.Products.ToListAsync();
	}

	public async Task<List<Product>> SearchProductsAsync(string keyword)
	{
		return await appDbContext.Products
					.AsNoTracking()
					.Where(p => EF.Functions.Collate(p.Name, "Latin1_General_CI_AI").Contains(keyword)
						&& p.QuantityInStock > 0).ToListAsync();
	}
	//public async Task<Product?> FindByIdAsync(Guid id)
	//{

	//    var product =  await appDbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
	//    if (product == null)
	//    {
	//        return null;
	//    }
	//    return product;
	//}

}