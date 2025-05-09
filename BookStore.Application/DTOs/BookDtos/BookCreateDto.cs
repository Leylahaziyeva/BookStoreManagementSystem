namespace BookStore.Application.DTOs.BookDtos
{
    public class BookCreateDto
    {
        public required string Title { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int AuthorId { get; set; }
        public int GenreId { get; set; }
    }
}
