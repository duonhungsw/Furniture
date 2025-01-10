namespace Furniture.Service.Mappings;

public class CustomerMappingProfile : Profile
{
	public CustomerMappingProfile()
	{
		CreateMap<SignInDTOs, Account>()
			.ForMember(c => c.Email, options => options.MapFrom(m => m.Email))
			.ForMember(c => c.HashPassword, options => options.MapFrom(m => m.HashPassword));

		CreateMap<SignupDTOs, Account>()
			.ForMember(c => c.Email, options => options.MapFrom(m => m.Email))
			.ForMember(c => c.Name, options => options.MapFrom(m => m.Name))
			.ForMember(c => c.HashPassword, options => options.MapFrom(m => m.Password));

		CreateMap<Account, AccountDto>()
			.ForMember(c => c.Email, options => options.MapFrom(m => m.Email))
			.ForMember(c => c.Name, options => options.MapFrom(m => m.Name))
			.ForMember(c => c.Avatar, options => options.MapFrom(m => m.Avatar))
			.ForMember(c => c.BirthDay, options => options.MapFrom(m => m.BirthDay))
			.ForMember(c => c.Phone, options => options.MapFrom(m => m.Phone))
			.ForMember(c => c.RoleName, options => options.MapFrom(m => m.RoleName));
	}
}
