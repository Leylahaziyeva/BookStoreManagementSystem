using BookStore.Application.DTOs.OrderDtos;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Infrastructure.EFCore.DataContext;

namespace BookStore.Application.Services
{
    public class OrderManager : CrudManager<Order, OrderDto, OrderCreateDto, OrderUpdateDto>, IOrderService
    {

    }
}