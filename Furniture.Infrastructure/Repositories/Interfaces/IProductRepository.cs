using Furniture.Core.Dtos.Product;

namespace Furniture.Infrastructure.Repositories.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<List<Product>> ListProductAsync();
    Task<List<Product>> SearchProductsAsync(string keyword);
}
