using BookStore.Application.DTOs.BookDtos;
using BookStore.Domain.Entities;

namespace BookStore.Application.Interfaces
{
    public interface IBookService : ICrudService<Book, BookDto, BookCreateDto, BookUpdateDto>
    {
        List<BookDto> GetBooksByAuthorId(int authorId);
        List<BookDto> GetBooksByGenreId(int genreId);
        List<BookDto> GetAll();
    }
}