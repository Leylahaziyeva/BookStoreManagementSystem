using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;

namespace BookStore.Infrastructure.EFCore.Repositories
{
    public class CustomerRepository : EfCoreRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository() : base() 
        {       
        }
    }
}
