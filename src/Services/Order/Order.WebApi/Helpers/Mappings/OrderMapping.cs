using AutoMapper;
using Order.Application.Dtos;
using Order.Domain.Entities;
using OrderClientGrpc;

namespace Order.WebApi.Helpers.Mappings
{
    public class OrderMapping : Profile
    {
        public OrderMapping()
        {
            CreateMap<OrderTicket, TicketOrderDto>()
                    .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.CustomerId))
                    .ReverseMap();

            CreateMap<OrderTicket, FullOrderDto>()
                    .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus))
                    .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
                    .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                    .ForMember(dest => dest.TicketDetails, opt => opt.Ignore())
                    .ReverseMap();

            CreateMap<OrderTicket, OrderDto>()
                    .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
                    .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus))
                    .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                    .ReverseMap();
        }
    }
}
