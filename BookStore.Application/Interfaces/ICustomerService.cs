using BookStore.Application.DTOs.CustomerDtos;
using BookStore.Domain.Entities;

namespace BookStore.Application.Interfaces
{
    public interface ICustomerService : ICrudService<Customer, CustomerDto, CustomerCreateDto, CustomerUpdateDto>
    {
    }
}