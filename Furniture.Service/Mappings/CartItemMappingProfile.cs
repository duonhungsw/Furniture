using Furniture.Core.Dtos.Cart;

namespace Furniture.Service.Mappings;

public class CartItemMappingProfile : Profile
{
    public CartItemMappingProfile() 
    {
        CreateMap<CartItemDto, CartItem>().ReverseMap();
    }
}
