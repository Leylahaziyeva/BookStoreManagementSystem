namespace BookStore.Domain.Entities
{
    public class Genre :Entity
    {
        public required string Name { get; set; }
        public List<Book> Books { get; set; } = new();
    }
}