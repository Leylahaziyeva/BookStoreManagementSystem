using AutoMapper;
using BookStore.Application.DTOs.AuthorDtos;
using BookStore.Application.DTOs.BookDtos;
using BookStore.Application.DTOs.CustomerDtos;
using BookStore.Application.DTOs.GenreDtos;
using BookStore.Application.DTOs.OrderDetailDtos;
using BookStore.Application.DTOs.OrderDtos;
using BookStore.Domain.Entities;

namespace BookStore.Application.AutoMapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<Author, AuthorDto>()
                .ForMember(x => x.FullName, opt => opt.MapFrom(src => $"{src.Name} {src.Surname}"))
                .ForMember(dest => dest.Books, opt => opt.MapFrom(src => src.Books)).ReverseMap();

            CreateMap<Author, AuthorCreateDto>().ReverseMap();
            CreateMap<Author, AuthorUpdateDto>().ReverseMap();


            CreateMap<Book, BookDto>()
                .ForMember(x => x.AuthorFullName, opt => opt.MapFrom(src => src.Author != null ? $"{src.Author.Name} {src.Author.Surname}" : null))
                .ForMember(dest => dest.GenreName, opt => opt.MapFrom(src => src.Genre != null ? src.Genre.Name : null)).ReverseMap();

            CreateMap<Book, BookCreateDto>().ReverseMap();
            CreateMap<Book, BookUpdateDto>().ReverseMap();


            CreateMap<Customer, CustomerDto>()
                .ForMember(x => x.FullName, opt => opt.MapFrom(src => $"{src.Name} {src.Surname}"))
                .ForMember(dest => dest.OrderCount, opt => opt.MapFrom(src => src.Orders.Count)).ReverseMap();

            CreateMap<Customer, CustomerCreateDto>().ReverseMap();
            CreateMap<Customer, CustomerUpdateDto>().ReverseMap();


            CreateMap<Genre, GenreDto>()
                .ForMember(x => x.BookCount, opt => opt.MapFrom(src => src.Books.Count))
                .ForMember(x => x.Books, opt => opt.MapFrom(src => src.Books)).ReverseMap();

            CreateMap<Genre, GenreCreateDto>().ReverseMap();
            CreateMap<Genre, GenreUpdateDto>().ReverseMap();

            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? $"{src.Customer.Name} {src.Customer.Surname}" : null))
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails)).ReverseMap();

            CreateMap<OrderCreateDto, Order>()
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails)).ReverseMap();

            CreateMap<Order, OrderUpdateDto>().ReverseMap();

            CreateMap<OrderDetail, OrderDetailDto>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book != null ? src.Book.Title : null))
                .ForMember(dest => dest.BookStock, opt => opt.MapFrom(src => src.Book != null ? src.Book.Stock : 0)).ReverseMap();


            CreateMap<OrderDetailDto, OrderDetail>()
                .ForMember(dest => dest.Book, opt => opt.Ignore())
                .ForMember(dest => dest.Order, opt => opt.Ignore());
        }
    }
}