using Furniture.Common.Exceptions;
using Furniture.Core.Enum;
using Microsoft.AspNetCore.Http;

namespace Furniture.Service.Services.Implements;

public class AccountServices(IAccountRepository accountRepository,
							ITokenService tokenService, IMapper mapper) : IAccountServices
{
	public Task<bool> ChangePassword(ChangePasswordDto changePasswordDto)
	{
		throw new NotImplementedException();
	}

	public async Task<AccountDto> GetAccountById(Guid Id)
	{
		var account = await accountRepository.GetByIdAsync(Id);
		if (account == null)
			throw new NotFoundException($"Not found customer with Id: {Id}");

		var result = mapper.Map<AccountDto>(account);
		return result;
	}

	public async Task<TokenDto?> LoginAsync(SignInDTOs model)
	{
		model.HashPassword = PasswordHasher.HashPasswordPBKDF2(model.HashPassword!);
		var account = mapper.Map<Account>(model);

		var result = await accountRepository.LoginAsync(account);
		if (result == null)
			throw new NotFoundException("Account does not exist.");

		var accessToken = tokenService.GenerateAccessToken(result);
		var refreshToken = tokenService.GenerateAccessToken(result);
		tokenService.SetTokensInsideCookie(accessToken, refreshToken);
		return new TokenDto
		{
			AccessToken = accessToken,
			RefreshToken = refreshToken
		};
	}

	public async Task<bool> RegisterAsync(SignupDTOs model)
	{
		var account = mapper.Map<Account>(model);
		account.HashPassword = PasswordHasher.HashPasswordPBKDF2(model.Password);
		account.RoleName = AppRoles.Customer.ToString();

        accountRepository.Create(account);
		if(await accountRepository.SaveChangesAsync())
		{
			return true;
		}
		return false;
	}

	public Task<bool> UpdateAsync(AccountDto customerDto, IFormFile avatar)
	{
		throw new NotImplementedException();
	}
}
