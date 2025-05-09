namespace BookStore.Domain.Entities
{
    public class Author : Entity
    {
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public List<Book> Books { get; set; } = new();
    }
}
