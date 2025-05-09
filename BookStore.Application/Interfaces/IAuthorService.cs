using BookStore.Application.DTOs.AuthorDtos;
using BookStore.Domain.Entities;

namespace BookStore.Application.Interfaces
{
    public interface IAuthorService : ICrudService<Author, AuthorDto, AuthorCreateDto, AuthorUpdateDto>
    {
        List<Author> GetAll();
    }
}