using Microsoft.EntityFrameworkCore;

namespace BookStore.Domain.Entities
{
    public class Order : Entity
    {
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public DateTime OrderDate { get; set; }

        [Precision(18, 4)]
        public decimal TotalPrice { get; set; }
        public List<OrderDetail> OrderDetails { get; set; } = new();
    }
}