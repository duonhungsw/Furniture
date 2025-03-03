namespace Furniture.API;

public class SignUpDtoValidator : AbstractValidator<SignupDTOs>
{
	public SignUpDtoValidator()
	{
		RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
		RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.")
			.EmailAddress().WithMessage("Invalid email format.");
		RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
	}
}
