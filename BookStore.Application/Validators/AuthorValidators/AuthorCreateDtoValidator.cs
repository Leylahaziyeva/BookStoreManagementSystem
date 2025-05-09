using BookStore.Application.DTOs.AuthorDtos;
using FluentValidation;

namespace BookStore.Application.Validators.AuthorValidators
{
    public class AuthorCreateDtoValidator : AbstractValidator<AuthorCreateDto>
    {
        public AuthorCreateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.").MaximumLength(20);

            RuleFor(x => x.Surname).NotEmpty().WithMessage("Surname is required.").MaximumLength(20);
        }
    }
}