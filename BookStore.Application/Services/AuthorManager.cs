using BookStore.Application.DTOs.AuthorDtos;
using BookStore.Application.DTOs.BookDtos;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Infrastructure.EFCore.DataContext;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Application.Services
{
    public class AuthorManager : CrudManager<Author, AuthorDto, AuthorCreateDto, AuthorUpdateDto>, IAuthorService
    {
        private readonly BookDbContext _context;

        public AuthorManager()
        {
            _context = new BookDbContext();
        }
        public List<AuthorDto> GetAll()
        {
            using (var context = new BookDbContext())
            {
                return context.Authors
                    .Include(a => a.Books)
                    .Select(a => new AuthorDto
                    {
                        Id = a.Id,
                        FullName = $"{a.Name} {a.Surname}",
                        Books = a.Books.Select(b => new BookDto
                        {
                            Id = b.Id,
                            Title = b.Title,
                            Price = b.Price,
                            Stock = b.Stock,
                            AuthorFullName = $"{a.Name} {a.Surname}",
                            GenreName = b.Genre != null ? b.Genre.Name : "Bilinmir"
                        }).ToList()
                    })
                    .ToList();
            }
        }
        List<Author> IAuthorService.GetAll()
        {
            return _context.Authors.Include(a => a.Books).ToList();
        }
    }
}