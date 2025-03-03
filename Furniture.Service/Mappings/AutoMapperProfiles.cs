using Furniture.Service.Mappings;

namespace Furniture.API.Mappings;

public class AutoMapperProfiles
{
	public static IMapper Initial()
	{
		var mapperConfiguration = new MapperConfiguration(cfg =>
		{
			cfg.AddProfile<CustomerMappingProfile>();
            cfg.AddProfile<ProductMappingProfile>();
			cfg.AddProfile<CartItemMappingProfile>();
        });

		return mapperConfiguration.CreateMapper();
	}
}
