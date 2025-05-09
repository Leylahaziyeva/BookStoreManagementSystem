using BookStore.Application.DTOs.CustomerDtos;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;

namespace BookStore.Application.Services
{
    public class CustomerManager : CrudManager<Customer, CustomerDto, CustomerCreateDto, CustomerUpdateDto>, ICustomerService
    {
    }
}