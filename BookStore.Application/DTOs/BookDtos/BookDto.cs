namespace BookStore.Application.DTOs.BookDtos
{
    public class BookDto
    {
        public int Id { get; set; }
        public string? Title { get; set; } 
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? AuthorFullName { get; set; }
        public string? GenreName { get; set; }
    }
}