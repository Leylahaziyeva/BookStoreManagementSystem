using BookStore.Application.DTOs.OrderDtos;
using FluentValidation;

namespace BookStore.Application.Validators.OrderValidators
{
    public class OrderUpdateDtoValidator : AbstractValidator<OrderUpdateDto>
    {
        public OrderUpdateDtoValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id is required for the update operation.");

            RuleFor(x => x.CustomerId).GreaterThan(0).WithMessage("CustomerId is required for the update operation.");

            RuleFor(x => x.OrderDate).NotEmpty().WithMessage("Order date is required.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Order date cannot be in the future.");

            RuleFor(x => x.TotalPrice).GreaterThanOrEqualTo(0).WithMessage("Total price must be non-negative.");
        }
    }
}