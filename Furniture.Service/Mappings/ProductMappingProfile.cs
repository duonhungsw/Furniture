namespace Furniture.Service;

public class ProductMappingProfile : Profile
{
	public ProductMappingProfile()
	{
		CreateMap<ProductDto, Product>().ReverseMap();
	}
}
