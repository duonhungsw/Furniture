using Furniture.Core.Dtos.Product;

namespace Furniture.Service.Services.Interfaces;

public interface IProductServices
{
    Task<Product?> GetProductByIdAsync(Guid Id);
    Task <bool> DeleteAsync(Guid Id);
    Task<bool> UpdateAsync(ProductDto model);
    Task<bool> CreateAsync(ProductDto model);
}
