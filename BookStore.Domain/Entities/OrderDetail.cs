using Microsoft.EntityFrameworkCore;

namespace BookStore.Domain.Entities
{
    public class OrderDetail :  Entity
    {
         public int Quantity { get; set; }

        [Precision(18, 4)]
        public decimal UnitPrice { get; set; } 
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public int BookId { get; set; }
        public Book? Book { get; set; } 
    }
}