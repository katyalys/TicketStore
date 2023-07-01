﻿using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Order.Application.Dtos;

namespace Order.WebApi.Helpers.Mappings
{
    public class TicketMapping : Profile
	{
		public TicketMapping()
		{
			CreateMap<OrderClientGrpc.TicketDto, TicketDetailInfo>()
                    .ForMember(dest => dest.Concert, opt => opt.MapFrom(src => src.Concert.Name))
                    .ForMember(dest => dest.Performer, opt => opt.MapFrom(src => src.Concert.Perfomer))
                    .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src.Concert.GenreName))
                    .ForMember(dest => dest.Place, opt => opt.MapFrom(src => $"{src.Concert.Place.City}, {src.Concert.Place.Street}, " +
                                                                             $"{src.Concert.Place.PlaceNmuber}"))
                    .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                    .ForMember(dest => dest.Sector, opt => opt.MapFrom(src => src.SectorName))
                    .ForMember(dest => dest.Row, opt => opt.MapFrom(src => src.Row))
                    .ForMember(dest => dest.Seat, opt => opt.MapFrom(src => src.Seat))
					.ReverseMap();

            CreateMap<DateTime, Timestamp>().ConvertUsing(src => Timestamp.FromDateTime(DateTime.SpecifyKind(src, DateTimeKind.Utc)));
            CreateMap<Timestamp, DateTime>().ConvertUsing(src => src.ToDateTime());
        }
	}
}