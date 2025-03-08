namespace Furniture.Web.Services;

public interface IAccountApi
{
    [Post("/accounts/login")]
    Task<TokenResponse?> LoginAsync(SignInDTOs model);

    [Post("/accounts/register")]
    Task<ApiResponse<bool>> RegisterAsync(SignupDTOs Regismodel);

    [Get("/accounts/get-by-id/{id}")]
    Task<AccountDto> GetAccountByIdAsync(Guid Id);

    [Post("/accounts/logout")]
    Task LogoutAsync();

    [Get("/accounts/user_info")]
    Task<ApiResponse<Account>?> GetUserInfoAsync();

    [Post("/accounts/forgot-password/email")]
    Task ForgotPasswordAsync(string email);

    [Post("/accounts/update-password")]
    Task<bool> UpdatePasswordAsync(ForgotPassDTOs forgotPasswordModel);

    [Multipart]
    [Put("/accounts/profile")]
    Task<bool> UpdateProfileAsync(
        [AliasAs("id")] Guid id,
        [AliasAs("name")] string? name,
        [AliasAs("birthDay")] string? birthDay,
        [AliasAs("phone")] string? phone,
        [AliasAs("avatar")] StreamPart avatar);

    [Patch("/accounts/changePhoneNumber")]
    Task<bool> UpdatePhoneNumber([Body] ChangePhoneNumberDto model);
}
