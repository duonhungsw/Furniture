using Furniture.Common.Exceptions;
using Furniture.Core.Dtos.Product;

namespace Furniture.Service.Services.Implements;
public class ProductServices(
    IProductRepository _repository,
    IMapper _mapper) : IProductServices
{
    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
            throw new NotFoundException($"Not found product with {id}");

        _repository.Delete(product);
        if (await _repository.SaveChangesAsync())
        {
            return true;
        }
        return false;
    }
    public async Task<Product?> GetProductByIdAsync(Guid id)
    {
        var result = await _repository.GetByIdAsync(id);
        return result;
    }
    public async Task<bool> UpdateAsync(ProductDto model)
    {
        var product = _mapper.Map<Product>(model);
        _repository.Update(product);
        await _repository.SaveChangesAsync();
        return true;
    }
    public async Task<bool> CreateAsync(ProductDto model)
    {
        var product = _mapper.Map<Product>(model);
        _repository.Create(product);
        await _repository.SaveChangesAsync();
        return true;
    }
}
