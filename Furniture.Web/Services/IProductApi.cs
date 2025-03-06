using Furniture.Common.Domain.Query;
using Furniture.Web.Models;

namespace Furniture.Web.Services;

public interface IProductApi
{
	[Get("/Product")]
	Task<PagedResult<ProductDto>> GetProductsAsync(
		[Query] int? PageIndex);
}
