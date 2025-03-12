namespace Furniture.Service;

public class StatusMappingProfile : Profile
{
	public StatusMappingProfile()
	{
		CreateMap<Status, StatusDto>();
	}
}
