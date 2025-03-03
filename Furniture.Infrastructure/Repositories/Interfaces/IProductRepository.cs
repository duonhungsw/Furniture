namespace Furniture.Infrastructure;

public interface IProductRepository : IGenericRepository<Product>
{
	Task<List<Product>> ListProductAsync();
	Task<List<Product>> SearchProductsAsync(string keyword);
	//Task<Product?> FindByIdAsync(Guid id);
}
