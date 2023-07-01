using AutoMapper;
using Catalog.Application.Dtos.ConcertDtos;
using Catalog.Domain.Entities;
using Google.Protobuf.WellKnownTypes;

namespace Catalog.WebApi.Helpers.Mappings
{
    public class ConcertMapping : Profile
    {
		public ConcertMapping()
		{
			CreateMap<Concert, ConcertsShortViewDto>()
					.ForMember(dest => dest.Place, opt => opt.MapFrom(src => src.Place))
					.ReverseMap();

			CreateMap<Concert, OrderServerGrpc.ConcertsShortViewDto>()
					.ForMember(dest => dest.Place, opt => opt.MapFrom(src => src.Place))
					.ReverseMap();

			CreateMap<DateTime, Timestamp>().ConvertUsing(src => Timestamp.FromDateTime(DateTime.SpecifyKind(src, DateTimeKind.Utc)));
			CreateMap<Timestamp, DateTime>().ConvertUsing(src => src.ToDateTime());

			CreateMap<Concert, FullInfoConcertDto>()
					.ForMember(dest => dest.Place, opt => opt.MapFrom(src => src.Place))
					.ReverseMap();
		}
	}
}
