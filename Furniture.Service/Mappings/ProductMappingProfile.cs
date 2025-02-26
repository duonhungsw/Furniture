
using Furniture.Core.Dtos.Product;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Furniture.Service.Mappings;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<ProductDto, Product>().ReverseMap();
    }
}
