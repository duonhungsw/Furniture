using Furniture.Core.Dtos.Order;

namespace Furniture.Service.Mappings;

public class OrderMappingProfile : Profile
{
	public OrderMappingProfile()
	{
		CreateMap<CreateOrderDto, Order>().ReverseMap();
	}
}
