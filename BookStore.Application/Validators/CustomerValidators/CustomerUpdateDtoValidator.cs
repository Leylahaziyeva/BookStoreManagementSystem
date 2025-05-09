using BookStore.Application.DTOs.CustomerDtos;
using FluentValidation;

namespace BookStore.Application.Validators.CustomerValidators
{
    public class CustomerUpdateDtoValidator : AbstractValidator<CustomerUpdateDto>
    {
        public CustomerUpdateDtoValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id is required for update.");

            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required for update.").MaximumLength(20).WithMessage("Name cannot exceed 50 characters.");

            RuleFor(x => x.Surname).NotEmpty().WithMessage("Surname is required for update.").MaximumLength(20).WithMessage("Surname cannot exceed 50 characters.");

            RuleFor(x => x.Email).EmailAddress().WithMessage("A valid email is required.").When(x => !string.IsNullOrEmpty(x.Email));
        }
    }
}