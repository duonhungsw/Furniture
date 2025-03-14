using Furniture.Web.Models;

namespace Furniture.Web.Services;

public interface IProductApi
{
	[Get("/Product")]
	Task<PagedResult<ProductDto>> GetProductsAsync(QueryInfo queryInfo);
	[Get("/Product/{id}")]
	Task<ProductDto> GetProductByIdAsync(Guid id);
	[Get("/Product/search")]
	Task<PagedResult<ProductDto>> SearchProductsAsync(QueryInfo queryInfo);
}
