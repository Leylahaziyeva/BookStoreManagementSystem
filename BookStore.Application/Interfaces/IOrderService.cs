using BookStore.Application.DTOs.OrderDtos;
using BookStore.Domain.Entities;

namespace BookStore.Application.Interfaces
{
    public interface IOrderService : ICrudService<Order, OrderDto, OrderCreateDto, OrderUpdateDto>
    {
    }
}