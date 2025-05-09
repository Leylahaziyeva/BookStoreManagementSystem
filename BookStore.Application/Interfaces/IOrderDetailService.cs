using BookStore.Application.DTOs.OrderDetailDtos;
using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace BookStore.Application.Interfaces
{
    public interface IOrderDetailService
    {
        OrderDetailDto Get(Expression<Func<OrderDetail, bool>> predicate, bool asNoTracking = false,
            Func<IQueryable<OrderDetail>, IIncludableQueryable<OrderDetail, object>>? include = null);
    }
}
