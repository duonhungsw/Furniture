namespace Furniture.Service;

public interface IProductServices
{
	//Task<Product?> GetProductByIdAsync(Guid Id);
	Task<bool> DeleteAsync(Guid Id);
	Task<bool> UpdateAsync(ProductDto model);
	Task<bool> CreateAsync(ProductDto model);
	Task<ProductDto?> GetProductByIdAsync(Guid Id);
	Task<List<ProductDto>> GetProductsAsync();
	Task<List<string>> GetBrandAsync();
	Task<List<string>> GetTypeAsync();
	Task<List<ProductDto>> SearchProductsAsync(string keyword);
    Task<List<ProductDto>> FilterProductsAsync(FilterProductInfo filterInfo);
}
