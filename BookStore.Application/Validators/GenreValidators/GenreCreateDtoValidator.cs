using BookStore.Application.DTOs.GenreDtos;
using FluentValidation;

namespace BookStore.Application.Validators.GenreValidators
{
    public class GenreCreateDtoValidator : AbstractValidator<GenreCreateDto>
    {
        public GenreCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Genre name cannot be empty.")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("The genre name must contain only letters and spaces.")
                .MaximumLength(20).WithMessage("\r\nThe genre name cannot exceed 20 characters.");
        }
    }
}