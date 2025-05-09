using BookStore.Application.DTOs.BookDtos;
using FluentValidation;

namespace BookStore.Application.Validators.BookValidators
{
    public class BookCreateDtoValidator : AbstractValidator<BookCreateDto>
    {
        public BookCreateDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.").MaximumLength(50).WithMessage("Title cannot exceed 50 characters."); 

            RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithMessage("Price must be non-negative.");

            RuleFor(x => x.Stock).GreaterThanOrEqualTo(0).WithMessage("Stock must be non-negative.");

            RuleFor(x => x.AuthorId).GreaterThan(0).WithMessage("Valid AuthorId is required.");

            RuleFor(x => x.GenreId).GreaterThan(0).WithMessage("Valid GenreId is required.");
        }
    }
}