using Furniture.Common;
using Furniture.Core.Enum;

namespace Furniture.Service.Services.Implements;

public class AccountServices(
	IAccountRepository _repository,
	ITokenService _tokenService,
	IFileStorageService _fileStorageService,
	IMapper _mapper) : IAccountServices
{
	private readonly string accountContainer = ContainerName.account.ToString();
	public Task<bool> ChangePassword(ChangePasswordDto changePasswordDto)
	{
		throw new NotImplementedException();
	}
	public async Task<AccountDto> GetAccountByEmail(string Email)
	{
		var account = await _repository.GetByEmailAsync(Email);
		var result = _mapper.Map<AccountDto>(account);
		return result;
	}
	public async Task<AccountDto> GetAccountById(Guid Id)
	{
		var account = await _repository.GetByIdAsync(Id);
		if (account == null)
			throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, "Account", Id));


		var result = _mapper.Map<AccountDto>(account);
		return result;
	}

	public async Task<List<AccountDto>> GetAccountsAsync()
		=> await _repository.GetAccountsAsync();

	public async Task<TokenDto?> LoginAsync(SignInDTOs model)
	{
		model.HashPassword = PasswordHasher.HashPasswordPBKDF2(model.HashPassword!);
		var account = _mapper.Map<Account>(model);

		var result = await _repository.LoginAsync(account);
		if (result == null)
			throw new NotFoundException(ErrorMessageBase.NotFound);


		var accessToken = _tokenService.GenerateAccessToken(result);
		var refreshToken = _tokenService.GenerateAccessToken(result);
		_tokenService.SetTokensInsideCookie(accessToken, refreshToken);
		return new TokenDto
		{
			AccessToken = accessToken,
			RefreshToken = refreshToken
		};
	}

	public async Task<bool> RegisterAsync(SignupDTOs model)
	{
		var account = _mapper.Map<Account>(model);
		account.HashPassword = PasswordHasher.HashPasswordPBKDF2(model.Password);
		account.RoleName = AppRoles.Customer.ToString();

		_repository.Create(account);
		if (await _repository.SaveChangesAsync())
		{
			return true;
		}
		return false;
	}

	public async Task<bool> ResetPasswordAsync(string Email, ForgotPassDTOs model)
	{
		var account = await _repository.GetByEmailAsync(Email);
		account!.HashPassword = PasswordHasher.HashPasswordPBKDF2(model.Password!);
		_repository.Update(account);
		if (await _repository.SaveChangesAsync())
		{
			return true;
		}
		return false;
	}

	public async Task<bool> UpdateAsync(UpdateAccountDto model)
	{
		var existingAccount = await _repository.GetByIdAsync(model.Id);
		if (existingAccount == null)
			throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, "Account", model.Id));
		bool isDeleted = await _fileStorageService.DeleteFileAsync(accountContainer, Path.GetFileName(existingAccount.Avatar!));

		var account = _mapper.Map(model, existingAccount);
		account.Avatar = await _fileStorageService.UploadFileAsync(accountContainer, model.Avatar!);
		account.BirthDay = model.BirthDay.ToString();

		_repository.Update(account);

		return await _repository.SaveChangesAsync() ? true : false;
	}
}
