using BookStore.Application.DTOs.OrderDtos;
using BookStore.Application.Validators.OrderDetailValidator;
using FluentValidation;

namespace BookStore.Application.Validators.OrderValidators
{
    public class OrderCreateDtoValidator : AbstractValidator<OrderCreateDto>
    {
        public OrderCreateDtoValidator()
        {
            RuleFor(x => x.CustomerId).GreaterThan(0).WithMessage("Please provide a valid Customer ID.");

            RuleFor(x => x.OrderDate).Must(date => date <= DateTime.Now).WithMessage("Order date can't be in the future.");

            RuleFor(x => x.TotalPrice).GreaterThanOrEqualTo(0).WithMessage("Total price must be non-negative.");

            RuleFor(x => x.OrderDetails).NotEmpty().WithMessage("At least one book must be included in the order.")
            .ForEach(detail => detail.SetValidator(new OrderDetailDtoValidator()));
        }
    }
}