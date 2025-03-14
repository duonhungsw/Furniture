namespace Furniture.Web.Services;

public interface IProductApi
{
    [Get("/products")]
    Task<PagedResult<ProductDto>> GetProductsAsync(
        [Query] int? PageIndex);

    [Multipart]
    [Post("/products/create")]
    Task<bool> Create(MultipartFormDataContent formData);
    [Patch("/products/update")]
    Task<bool> Update(MultipartFormDataContent formData);

    [Delete("/products/delete/{id}")]
    Task<bool> Delete(Guid id);

    [Get("/products/{id}")]
    Task<ProductDto> GetProductById(Guid id);
}
