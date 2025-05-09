using BookStore.Application.DTOs.CustomerDtos;
using FluentValidation;

namespace BookStore.Application.Validators.CustomerValidators
{
    public class CustomerCreateDtoValidator : AbstractValidator<CustomerCreateDto>
    {
        public CustomerCreateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.").MaximumLength(20);

            RuleFor(x => x.Surname).NotEmpty().WithMessage("Surname is required.").MaximumLength(20);

            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.").EmailAddress().WithMessage("A valid email is required.");
        }

    }
}