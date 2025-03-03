namespace Furniture.API;

public class SignInDtoValidator : AbstractValidator<SignInDTOs>
{
	public SignInDtoValidator()
	{
		RuleFor(x => x.Email).NotEmpty().WithMessage("Name or email is required.");
		RuleFor(x => x.HashPassword).NotEmpty().WithMessage("Password is required.");
	}
}
