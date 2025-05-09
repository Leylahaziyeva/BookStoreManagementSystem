namespace BookStore.Application.DTOs.OrderDetailDtos
{
    public class OrderDetailDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int BookId { get; set; }
        public string? BookTitle { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int BookStock { get; set; }
        public decimal Total => Quantity * UnitPrice;
    }
}