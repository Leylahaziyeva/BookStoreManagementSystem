using BookStore.Application.DTOs.AuthorDtos;
using FluentValidation;

namespace BookStore.Application.Validators.AuthorValidators
{
    public class AuthorUpdateDtoValidator : AbstractValidator<AuthorUpdateDto>
    {
        public AuthorUpdateDtoValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id is required for update.");

            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.").MaximumLength(20); ;

            RuleFor(x => x.Surname).NotEmpty().WithMessage("Surname is required.").MaximumLength(20); ;
        }
    }
}
