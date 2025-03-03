namespace Furniture.Service;

public class AutoMapperProfiles
{
	public static IMapper Initial()
	{
		var mapperConfiguration = new MapperConfiguration(cfg =>
		{
			cfg.AddProfile<AccountMappingProfile>();
			cfg.AddProfile<ProductMappingProfile>();
			cfg.AddProfile<CartItemMappingProfile>();
		});

		return mapperConfiguration.CreateMapper();
	}
}
