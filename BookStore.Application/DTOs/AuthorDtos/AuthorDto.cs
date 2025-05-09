using BookStore.Application.DTOs.BookDtos;

namespace BookStore.Application.DTOs.AuthorDtos
{
    public class AuthorDto
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public List<BookDto> Books { get; set; } = new();
    }
}