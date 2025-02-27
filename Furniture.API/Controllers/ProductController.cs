using AutoMapper;
using Furniture.Core.Dtos.Product;
using Microsoft.AspNetCore.Mvc;

namespace Furniture.API.Controllers;
public class ProductController(IProductServices _services
    , IFileStorageService _storageService, IMapper _mapper) : BaseApiController
{
    [HttpPatch("update")]
    public async Task<bool> Update([FromForm] ProductDto model)
    {
        bool result = await _services.UpdateAsync(model);
        return result;
    }
    [HttpDelete("delete/{id}")]
    public async Task<bool> Delete([FromRoute] Guid id)
    {

        var result = await _services.DeleteAsync(id);
        return result;
    }
    [HttpPost]
    public async Task<bool> Create([FromForm] ProductDto model)
    {
        var result = await _services.CreateAsync(model);
        return result;
    }
}
