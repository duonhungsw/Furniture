namespace Furniture.Web.Services;

public interface IAccountService
{
    [Post("/login")]
    Task<TokenDto?> LoginAsync(SignInDTOs model);

    [Post("/register")]
    Task<bool> RegisterAsync(SignupDTOs Regismodel);

    [Get("/customer/get-by-id/{id}")]
    Task<AccountDto> GetAccountById(Guid Id);

    [Delete("/logout")]
    Task Logout();

    [Get("/user_info")]
    Task<Account> GetUserInfo();

    Task<bool> ChangePassword(ChangePasswordDto changePasswordDto);
    Task<bool> UpdateAsync(AccountDto customerDto, IFormFile avatar);
}
