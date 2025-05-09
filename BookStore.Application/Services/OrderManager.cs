using BookStore.Application.DTOs.OrderDtos;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;

namespace BookStore.Application.Services
{
    public class OrderManager : CrudManager<Order, OrderDto, OrderCreateDto, OrderUpdateDto>, IOrderService
    {


    }
}