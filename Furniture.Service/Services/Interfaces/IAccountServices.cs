using Microsoft.AspNetCore.Mvc;

namespace Furniture.Service;

public interface IAccountServices
{
	Task<TokenResponse?> LoginAsync(SignInDTOs model);
	Task<TokenResponse?> LoginGoogleAsync(Account model);
	Task<bool> RegisterAsync(SignupDTOs Regismodel);
	Task<AccountDto> GetAccountByIdAsync(Guid Id);
	Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
	Task<bool> UpdateAsync(UpdateAccountDto model);
	Task<AccountDto> GetAccountByEmailAsync(string Email);
	Task<bool> ResetPasswordAsync(string Email, ForgotPassDTOs Regismodel);
	Task<List<AccountDto>> GetAccountsAsync();
	Task<bool> UpdatePhoneNumberAsync(Guid accountId, string phoneNumber);
	Task<Guid?> GetAccountIdAsync();
	Task<bool> HandleAccountAction([FromBody] AccountActionDto request);
}
