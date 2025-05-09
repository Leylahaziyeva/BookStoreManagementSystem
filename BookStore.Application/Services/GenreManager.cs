using BookStore.Application.DTOs.GenreDtos;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Application.Services
{
    public class GenreManager : CrudManager<Genre, GenreDto, GenreCreateDto, GenreUpdateDto>, IGenreService
    {
        public GenreManager() : base() { }
        public List<GenreDto> GetAllWithBooks()
        {
            return GetAll(include: query => query.Include(g => g.Books).ThenInclude(b => b.Author));
        }
    }
}