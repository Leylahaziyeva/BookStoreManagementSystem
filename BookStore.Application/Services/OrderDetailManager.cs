using AutoMapper;
using BookStore.Application.AutoMapping;
using BookStore.Application.DTOs.OrderDetailDtos;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Infrastructure.EFCore.Repositories;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace BookStore.Application.Services;
public class OrderDetailManager : IOrderDetailService
{
    private readonly IOrderDetailRepository _repository;
    private readonly IMapper _mapper;

    public OrderDetailManager()
    {
        _repository = new OrderDetailRepository();

        var config = new MapperConfiguration(cfg => {
            cfg.AddProfile<MappingProfile>();

        });

        _mapper = config.CreateMapper();
    }

    public OrderDetailDto Get(Expression<Func<OrderDetail, bool>> predicate, bool asNoTracking = false,
            Func<IQueryable<OrderDetail>, IIncludableQueryable<OrderDetail, object>>? include = null)
    {
        var orderDetail = _repository.Get(predicate, asNoTracking, include);

        var orderDetailsDto = _mapper.Map<OrderDetailDto>(orderDetail);

        return orderDetailsDto;
    }
}