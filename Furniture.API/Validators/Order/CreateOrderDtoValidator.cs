namespace Furniture.API.Validators.Order;
public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
	public CreateOrderDtoValidator()
	{
		//RuleFor(x => x.City).NotEmpty().WithMessage(ErrorMessageBase.Required);
		//RuleFor(x => x.District).NotEmpty().WithMessage(ErrorMessageBase.Required);
		//RuleFor(x => x.Town).NotEmpty().WithMessage(ErrorMessageBase.Required);
		//RuleFor(x => x.Detail).NotEmpty().WithMessage(ErrorMessageBase.Required);

		//RuleFor(x => x.Phone)
		//.NotEmpty().WithMessage(ErrorMessageBase.Required)
		//.Length(10, 11)
		//.WithMessage($"{ErrorMessageBase.InvalidPhoneNumber} {ErrorMessageBase.Range}");


		//RuleFor(x => x.Note).NotEmpty().WithMessage(ErrorMessageBase.Required);
		//RuleFor(x => x.PaymentMethod).NotEmpty().WithMessage(ErrorMessageBase.Required);
	}
}
