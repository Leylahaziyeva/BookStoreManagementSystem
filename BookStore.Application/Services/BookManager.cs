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

        public override BookDto Update(BookUpdateDto dto)
        {
            var book = _context.Books
                .FirstOrDefault(b => b.Id == dto.Id);

            if (book == null)
                throw new Exception("Book not found.");

            book.Title = dto.Title;
            book.Price = dto.Price;
            book.Stock = dto.Stock;
            book.AuthorId = dto.AuthorId;
            book.GenreId = dto.GenreId;

            _context.SaveChanges();

            _context.Entry(book).Reference(b => b.Author).Load();
            _context.Entry(book).Reference(b => b.Genre).Load();

            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                AuthorFullName = $"{book.Author.Name} {book.Author.Surname}",
                GenreName = book.Genre.Name,
                Price = book.Price,
                Stock = book.Stock
            };
        }

        public List<BookDto> GetAll()
        {
            var books = _context.Books
                .Include(b => b.Author)   
                .Include(b => b.Genre)   
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    AuthorFullName = b.Author != null ? $"{b.Author.Name} {b.Author.Surname}" : "Məlumat yoxdur",
                    GenreName = b.Genre != null ? b.Genre.Name : "Məlumat yoxdur",
                    Price = b.Price,
                    Stock = b.Stock
                })
                .ToList();
                 return books;
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
