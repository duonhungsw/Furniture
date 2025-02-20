namespace Furniture.Web.Services;

public interface IAccountApi
{
    [Post("/login")]
    Task<TokenDto?> LoginAsync(SignInDTOs model);

    [Post("/register")]
    Task<ApiResponse<bool>> RegisterAsync(SignupDTOs Regismodel);

    [Get("/customer/get-by-id/{id}")]
    Task<AccountDto> GetAccountById(Guid Id);

    [Post("/logout")]
    Task Logout();

    [Get("/user_info")]
    Task<ApiResponse<Account>?> GetUserInfo();

    [Post("/forgot-password/email")]
    Task ForgotPassword(string email);

    [Post("/update-password")]
    Task<bool> UpdatePassword(ForgotPassDTOs forgotPasswordModel);

    //Task<bool> ChangePassword(ChangePasswordDto changePasswordDto);
    //Task<bool> UpdateAsync(AccountDto customerDto, IFormFile avatar);
}
