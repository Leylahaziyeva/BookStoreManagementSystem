namespace BookStore.Application.DTOs.OrderDtos
{
    public class OrderUpdateDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
