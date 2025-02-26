using FluentValidation;
using Furniture.Core.Dtos.Order;

namespace Furniture.API.Validators.Order;
public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
	public CreateOrderDtoValidator()
	{
		RuleFor(x => x.Country).NotEmpty().WithMessage("Country is required.");
		RuleFor(x => x.City).NotEmpty().WithMessage("City is required.");
		RuleFor(x => x.District).NotEmpty().WithMessage("District is required.");
		RuleFor(x => x.Town).NotEmpty().WithMessage("Town is required.");
		RuleFor(x => x.Detail).NotEmpty().WithMessage("Address detail is required.");

		RuleFor(x => x.Phone)
			.NotEmpty().WithMessage("Phone number is required.")
			.Matches(@"^\d{10,11}$").WithMessage("Phone number must be 10 to 11 digits long.");

		RuleFor(x => x.Note).NotEmpty().WithMessage("Note is required.");
		RuleFor(x => x.PaymentMethod).NotEmpty().WithMessage("Payment method is required.");
	}
}
