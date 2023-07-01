using AutoMapper;
using Catalog.Application.Dtos.TicketDtos;
using Catalog.Domain.Entities;
using OrderServerGrpc;

namespace Catalog.WebApi.Helpers.Mappings
{
    public class TicketMapping : Profile
    { 
        public TicketMapping()
        {
            CreateMap<Ticket, TicketAddDto>()
                .ReverseMap();

            CreateMap<Ticket, Application.Dtos.TicketDtos.TicketDto>()
              .ForMember(dest => dest.Concert, opt => opt.MapFrom(src => src.Concert))
              .ForMember(dest => dest.SectorName, opt => opt.MapFrom(src => src.Sector.Name.ToString()))
              .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Sector.Price))
              .ReverseMap();

            CreateMap<Ticket, OrderServerGrpc.TicketDto>()
                .ForMember(dest => dest.Concert, opt => opt.MapFrom(src => src.Concert))
                .ForMember(dest => dest.SectorName, opt => opt.MapFrom(src => src.Sector.Name.ToString()))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Sector.Price))
                .ReverseMap();
        }
    }
}
