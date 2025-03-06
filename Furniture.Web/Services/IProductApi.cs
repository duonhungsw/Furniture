using Furniture.Web.Models;

namespace Furniture.Web.Services
{
	public interface IProductApi
	{
		[Get("/Product")]
		Task<ProductPagingModel> GetProductsAsync(
			[Query] int? PageIndex = 1,
			[Query] int? PageSize = 8);
	}
}
