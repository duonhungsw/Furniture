using AutoMapper;
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
    public async Task<ProductDto?> GetProductByIdAsync(Guid id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
            throw new NotFoundException($"Not found product with Id: {id}");

        var result = _mapper.Map<ProductDto>(product);
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

    public async Task<List<ProductDto>> GetProductsAsync()
    {
        var products = await _repository.GetAllAsync();
        return _mapper.Map<List<ProductDto>>(products);
    }

    public async Task<List<ProductDto>> SearchProductsAsync(string keyword)
    {
        var products = await _repository.SearchProductsAsync(keyword);
        return _mapper.Map<List<ProductDto>>(products);
    }
}
