using Furniture.Web.Models;

namespace Furniture.Web.Services;

public interface IProductApi
{
	[Get("/products")]
	Task<PagedResult<ProductDto>> GetProductsAsync(
		[Query] int? PageIndex);
<<<<<<< Updated upstream
    [Multipart]
    [Post("/products/create")]
    Task<bool> Create([FromForm] MultipartFormDataContent formData);
    [Patch("/products/update")]
    Task<bool> Update([Body] MultipartFormDataContent formData);

    [Delete("/products/delete/{id}")]
    Task<bool> Delete(Guid id);

    [Get("/products/{id}")]
    Task<ProductDto> GetProductById(Guid id);
=======
    [Get("/products")]
    Task<PagedResult<ProductDto>> GetProductsWithPaging([Query] QueryInfo queryInfo);
>>>>>>> Stashed changes
}
