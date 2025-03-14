
using Furniture.Core;
using Microsoft.AspNetCore.Mvc;
namespace Furniture.Service;

public class AccountServices(
	IAccountRepository _repository,
	ITokenService _tokenService,
	IFileStorageService _fileStorageService,
	ICartRepository _cartRepository,
	IMapper _mapper) : IAccountServices
{
	private readonly string accountContainer = ContainerName.account.ToString();
	public Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
	{
		throw new NotImplementedException();
	}
	public async Task<AccountDto> GetAccountByEmailAsync(string Email)
	{

		var account = await _repository.GetByEmailAsync(Email);
		var result = _mapper.Map<AccountDto>(account);
		return result;
	}
	public async Task<AccountDto> GetAccountByIdAsync(Guid Id)
	{
		var account = await _repository.GetByIdAsync(Id);
		if (account == null)
			throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, "Account", Id));


		var result = _mapper.Map<AccountDto>(account);
		return result;
	}

	public Task<AccountDto> GetAccountByPasswordAsync(Guid id, string password)
	{
		throw new NotImplementedException();
	}

	public async Task<Guid?> GetAccountIdAsync()
	{
		var account = await _tokenService.GetTokenAsync();
		return account?.Id;
	}
	public async Task<List<AccountDto>> GetAccountsAsync()
		=> await _repository.GetAccountsAsync();

	public async Task<TokenResponse?> LoginAsync(SignInDTOs model)
	{
		model.HashPassword = PasswordHasher.HashPasswordPBKDF2(model.HashPassword!);
		var account = _mapper.Map<Account>(model);

		var result = await _repository.LoginAsync(account);
		if (result == null)
			throw new NotFoundException(ErrorMessageBase.NotFound);


		var accessToken = _tokenService.GenerateAccessToken(result);
		var refreshToken = _tokenService.GenerateAccessToken(result);
		_tokenService.SetTokensInsideCookie(accessToken, refreshToken);
		return new TokenResponse
		{
			AccessToken = accessToken,
			RefreshToken = refreshToken
		};
	}
	public async Task<TokenResponse?> LoginGoogleAsync(Account model)
	{
		var accessToken = _tokenService.GenerateAccessToken(model);
		var refreshToken = _tokenService.GenerateAccessToken(model);
		_tokenService.SetTokensInsideCookie(accessToken, refreshToken);
		return new TokenResponse
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


		var cartModel = new Cart
		{
			AccountId = account.Id,
			CartTotal = 0
		};
		_cartRepository.Create(cartModel);

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
		var oldAvatar = existingAccount.Avatar;
        var account = _mapper.Map(model, existingAccount);
        if (model.Avatar == null)
        {
            account.Avatar = oldAvatar;
            account.BirthDay = model.BirthDay;

            _repository.Update(account);
            return await _repository.SaveChangesAsync() ? true : false;
        }
        bool isDeleted = await _fileStorageService.DeleteFileAsync(accountContainer, Path.GetFileName(existingAccount.Avatar!));
        account.Avatar = await _fileStorageService.UploadFileAsync(accountContainer, model.Avatar!);
        account.BirthDay = model.BirthDay;

        _repository.Update(account);
        return await _repository.SaveChangesAsync() ? true : false;
    }
    public async Task<bool> UpdatePhoneNumberAsync(Guid accountId, string phoneNumber)
    {
        var account = await _repository.GetByIdAsync(accountId);
        if (account == null)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, "Account", accountId));
        account.Phone = phoneNumber;
        _repository.Update(account);
        return await _repository.SaveChangesAsync() ? true : false;
    }
    public async Task<bool> HandleAccountAction([FromBody] AccountActionDto request)
    {
        var account = await _repository.GetByIdAsync(request.Id);
        if (account == null)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, "Account", request.Id));

        if (request.Action == AccountAction.VerifyByPassword.ToString())
        {
            return await _repository.VerifyAccountByPasswordAsync(account.Id, PasswordHasher.HashPasswordPBKDF2(request.Password!));
        }
        if (request.Action == AccountAction.ChangeNumber.ToString())
        {
            account.Phone = request.NewPhoneNumber;
            _repository.Update(account);
            return await _repository.SaveChangesAsync() ? true : false;
        }
        if (request.Action == AccountAction.ChangeEmail.ToString())
        {
            account.Email = request.NewEmail!;
            _repository.Update(account);
            return await _repository.SaveChangesAsync() ? true : false;
        }
        if (request.Action == AccountAction.ChangePassword.ToString())
        {
            account.HashPassword = PasswordHasher.HashPasswordPBKDF2(request.NewPassword!);
            _repository.Update(account);
            return await _repository.SaveChangesAsync() ? true : false;
        }
        return false;
    }
    public async Task<bool> UpdateRoleAsync(AccountDto model)
    {
        var existingAccount = await _repository.GetByIdAsync(model.Id);
        if (existingAccount == null) return false;
        existingAccount.RoleName = model.RoleName!;
        _repository.Update(existingAccount);
        return await _repository.SaveChangesAsync();
    }
}
