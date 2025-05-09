using BookStore.Application.DTOs.OrderDetailDtos;
using FluentValidation;

namespace BookStore.Application.Validators.OrderDetailValidator
{
    public class OrderDetailDtoValidator : AbstractValidator<OrderDetailDto>
    {
        public OrderDetailDtoValidator()
        {
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("The quantity cannot be less than 0.")
                .Must((dto, quantity) => quantity <= dto.BookStock)
                .WithMessage("The quantity cannot exceed the available book stock.");
        }
    }
}
