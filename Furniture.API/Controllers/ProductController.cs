using AutoMapper;
using Furniture.Core.Dtos.Product;
using Microsoft.AspNetCore.Mvc;

namespace Furniture.API.Controllers;
public class ProductController(IProductServices _services, IMapper _mapper) : BaseApiController
{
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto?>> GetProductById(Guid id)
    {
        var product = await _services.GetProductByIdAsync(id);
        return Ok(product);
    }
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductDto>>> GetProductsWithPaging([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 8)
    {
        var result = await _services.GetProductsAsync();
        return CreatePagedResult(result, pageIndex, pageSize);
    }
    [HttpGet("search")]
    public async Task<ActionResult<PagedResult<ProductDto>>> SearchProductsWithPaging([FromQuery] string keyWord, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 8)
    {
        var result = await _services.SearchProductsAsync(keyWord);
        return CreatePagedResult(result, pageIndex, pageSize);
    }
    [HttpPatch("update")]
    public async Task<bool> Update([FromBody] ProductDto model)
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
    public async Task<bool> Create([FromBody] ProductDto model)
    {
        var result = await _services.CreateAsync(model);
        return result;
    }
}
