namespace Furniture.Service;

public class OrderMappingProfile : Profile
{
	public OrderMappingProfile()
	{
		CreateMap<CreateOrderDto, Order>().ReverseMap();
		CreateMap<CreateOrderItemDto, OrderItem>().ReverseMap();
	}
}
