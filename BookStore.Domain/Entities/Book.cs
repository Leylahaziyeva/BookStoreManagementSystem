using Microsoft.EntityFrameworkCore;

namespace BookStore.Domain.Entities
{
    public class Book : Entity
    {
        public required string Title { get; set; }

        [Precision(18, 4)]
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
        public int GenreId { get; set; } 
        public Genre? Genre { get; set; }
        public List<OrderDetail> OrderDetails { get; set; } = new();
    }
}