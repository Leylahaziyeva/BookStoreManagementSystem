namespace BookStore.Domain.Entities
{
    public class Customer : Entity
    {
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string Email { get; set; }
        public List<Order> Orders { get; set; } = new();
    }
}