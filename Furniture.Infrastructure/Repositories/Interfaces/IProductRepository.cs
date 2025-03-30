namespace Furniture.Infrastructure;

public interface IProductRepository : IGenericRepository<Product>
{
	Task<List<Product>> ListProductAsync();
	Task<List<Product>> SearchProductsAsync(string keyword);
	Task<List<string>> GetBrandAsync();
	Task<List<string>> GetTypeAsync();
    Task<List<Product>> FilterProductsAsync(FilterProductInfo filterInfo);
    //Task<Product?> FindByIdAsync(Guid id);
    Task<bool> IsImageUsedByOtherProductsAsync(string imageUrl, Guid productId);
}
