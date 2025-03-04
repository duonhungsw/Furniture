namespace Furniture.Service;

public interface IAccountServices
{
	Task<TokenResponse?> LoginAsync(SignInDTOs model);
	Task<bool> RegisterAsync(SignupDTOs Regismodel);
	Task<AccountDto> GetAccountByIdAsync(Guid Id);
	Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
	Task<bool> UpdateAsync(UpdateAccountDto model);
	Task<AccountDto> GetAccountByEmailAsync(string Email);
	Task<bool> ResetPasswordAsync(string Email, ForgotPassDTOs Regismodel);
	Task<List<AccountDto>> GetAccountsAsync();
}
