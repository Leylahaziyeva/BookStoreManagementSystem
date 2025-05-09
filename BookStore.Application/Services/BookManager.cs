using BookStore.Application.DTOs.BookDtos;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Infrastructure.EFCore.DataContext;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Application.Services
{
    public class BookManager : CrudManager<Book, BookDto, BookCreateDto, BookUpdateDto>, IBookService
    {
        private readonly BookDbContext _context;
        public BookManager()
        {
            _context = new BookDbContext();
        }
        public List<BookDto> GetBooksByAuthorId(int authorId)
        {
            return _context.Books
                  .AsNoTracking()
                .Include(b => b.Author)   
                .Include(b => b.Genre)  
                .Where(b => b.AuthorId == authorId)
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Price = b.Price,
                    Stock = b.Stock,
                    AuthorFullName = b.Author != null ? $"{b.Author.Name} {b.Author.Surname}" : "Bilinmir",
                    GenreName = b.Genre != null ? b.Genre.Name : "Bilinmir"
                })
                .ToList();
        }

        public List<BookDto> GetBooksByGenreId(int genreId)
        {
            return _context.Books
                  .AsNoTracking()
                .Include(b => b.Author)   
                .Include(b => b.Genre)  
                .Where(b => b.GenreId == genreId)
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Price = b.Price,
                    Stock = b.Stock,
                    AuthorFullName = b.Author != null ? $"{b.Author.Name} {b.Author.Surname}" : "Bilinmir",
                    GenreName = b.Genre != null ? b.Genre.Name : "Bilinmir"
                })
                .ToList();
        }
    }
}
