using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;

namespace BookStore.Infrastructure.EFCore.Repositories
{
    public class OrderRepository : EfCoreRepository<Order>, IOrderRepository
    {
        public OrderRepository() : base()
        {       
        }
    }
}