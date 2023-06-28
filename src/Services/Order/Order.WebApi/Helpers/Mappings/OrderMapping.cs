using AutoMapper;
using Order.Domain.Entities;
using Order.Application.Dtos;

namespace Order.WebApi.Helpers.Mappings
{
	public class OrderMapping : Profile
	{
		public OrderMapping()
		{
			CreateMap<OrderTicket, TicketOrderDto>()
					.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.CustomerId))
					.ReverseMap();
		}
	}
}
