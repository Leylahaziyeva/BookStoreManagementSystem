using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;

namespace BookStore.Infrastructure.EFCore.Repositories
{
    public class OrderDetailRepository : EfCoreRepository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository() : base()
        {         
        }
    }
}