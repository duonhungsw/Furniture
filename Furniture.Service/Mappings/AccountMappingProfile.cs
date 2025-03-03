namespace Furniture.Service.Mappings;

public class AccountMappingProfile : Profile
{
	public AccountMappingProfile()
	{
		CreateMap<SignInDTOs, Account>()
			.ForMember(c => c.HashPassword, options => options.MapFrom(m => m.HashPassword));

		CreateMap<SignupDTOs, Account>()
			.ForMember(c => c.HashPassword, options => options.MapFrom(m => m.Password));

		CreateMap<Account, AccountDto>().ReverseMap();
		CreateMap<Account, UpdateAccountDto>().ReverseMap();
	}
}
