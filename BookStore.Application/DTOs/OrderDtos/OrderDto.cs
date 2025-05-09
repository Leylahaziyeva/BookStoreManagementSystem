using BookStore.Application.DTOs.OrderDetailDtos;

namespace BookStore.Application.DTOs.OrderDtos
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; } = new();
    }
}
