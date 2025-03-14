namespace Furniture.Core;

public class ProductDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Product name cannot be empty.")]
    [MaxLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Product description cannot be empty.")]
    [MaxLength(500, ErrorMessage = "Product description cannot exceed 500 characters.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Product price cannot be empty.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Product price must be greater than 0.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "At least one image must be uploaded.")]
    public List<IFormFile>? Images { get; set; }

    //[Url(ErrorMessage = "Invalid URL format.")]
    public string? PictureUrl { get; set; }

    [Required(ErrorMessage = "Product type cannot be empty.")]
    public string? Type { get; set; }

    [Required(ErrorMessage = "Brand cannot be empty.")]
    public string? Brand { get; set; }

    [Required(ErrorMessage = "Stock quantity cannot be empty.")]
    [Range(1, int.MaxValue, ErrorMessage = "Stock quantity must be greater than 0.")]
    public int QuantityInStock { get; set; }

}
