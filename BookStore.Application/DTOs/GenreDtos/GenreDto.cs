using BookStore.Application.DTOs.BookDtos;

namespace BookStore.Application.DTOs.GenreDtos
{
    public class GenreDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int BookCount { get; set; }
        public required List<BookDto> Books { get; set; }
    }
}
