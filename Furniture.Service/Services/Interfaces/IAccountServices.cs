using Furniture.Core;

namespace Furniture.Service.Services.Interfaces;

public interface IAccountServices
{
	Task<TokenDto?> LoginAsync(SignInDTOs model);
	Task<bool> RegisterAsync(SignupDTOs Regismodel);
	Task<AccountDto> GetAccountById(Guid Id);
	Task<bool> ChangePassword(ChangePasswordDto changePasswordDto);
	Task<bool> UpdateAsync(UpdateAccountDto model);
	Task<AccountDto> GetAccountByEmail(string Email);
	Task<bool> ResetPasswordAsync(string Email, ForgotPassDTOs Regismodel);
	Task<List<AccountDto>> GetAccountsAsync();
}
