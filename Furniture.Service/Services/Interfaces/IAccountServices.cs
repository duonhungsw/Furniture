using Furniture.Core.DTOs;
using Microsoft.AspNetCore.Http;

namespace Furniture.Service.Services.Interfaces;

public interface IAccountServices
{
	Task<TokenDto?> LoginAsync(SignInDTOs model);
	Task<bool> RegisterAsync(SignupDTOs Regismodel);
	Task<AccountDto> GetAccountById(Guid Id);
	Task<bool> ChangePassword(ChangePasswordDto changePasswordDto);
	Task<bool> UpdateAsync(AccountDto customerDto, IFormFile avatar);
}
