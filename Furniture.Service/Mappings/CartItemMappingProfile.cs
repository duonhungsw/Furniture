namespace Furniture.Service;

public class CartItemMappingProfile : Profile
{
	public CartItemMappingProfile()
	{
		CreateMap<CartItemDto, CartItem>().ReverseMap();
	}
}
