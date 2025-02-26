namespace Furniture.Core.Dtos.Product;
using Microsoft.AspNetCore.Http;
public class ProductDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? PictureUrl { get; set; }
    public string? Type { get; set; }
    public string? Brand { get; set; }
    public int QuantityInStock { get; set; }
    public List<IFormFile> Images { get; set; }
}