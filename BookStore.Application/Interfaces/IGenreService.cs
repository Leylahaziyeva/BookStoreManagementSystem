using BookStore.Application.DTOs.GenreDtos;
using BookStore.Domain.Entities;

namespace BookStore.Application.Interfaces
{
    public interface IGenreService : ICrudService<Genre, GenreDto, GenreCreateDto, GenreUpdateDto>
    {
        List<GenreDto> GetAllWithBooks();
    }
}