using Microsoft.AspNetCore.Mvc;

namespace Furniture.API.Controllers;
[Route("products")]
public class ProductController(IProductServices _services) : BaseApiController
{
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto?>> GetProductById(Guid id)
    {
        var product = await _services.GetProductByIdAsync(id);
        return product;
    }
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductDto>>> GetProductsWithPaging([FromQuery] QueryInfo queryInfo)
    {
        var result = await _services.GetProductsAsync();
        return CreatePagedResult(result, queryInfo);
    }
    [HttpGet("search")]
    public async Task<ActionResult<PagedResult<ProductDto>>> SearchProductsWithPaging([FromQuery] QueryInfo queryInfo)
    {
        var result = await _services.SearchProductsAsync(queryInfo.SearchText!);
        return CreatePagedResult(result, queryInfo);
    }
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
    [HttpPost("create")]
    public async Task<bool> Create([FromForm] ProductDto model)
    {
        var result = await _services.CreateAsync(model);
        return result;
    }
}
