using BookStore.Application.DTOs.OrderDetailDtos;

namespace BookStore.Application.DTOs.OrderDtos
{
    public class OrderCreateDto
    {
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; } = new();
    }
}