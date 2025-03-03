namespace Furniture.API;

public class UpdateAccountDtoValidator : AbstractValidator<UpdateAccountDto>
{
	public UpdateAccountDtoValidator()
	{
		RuleFor(x => x.Avatar)
			.NotNull().WithMessage(ErrorMessageBase.Required)
			.Must(file => file.Length <= 2 * 1024 * 1024)
			.WithMessage("Avatar size must not exceed 2MB.");
	}
}
