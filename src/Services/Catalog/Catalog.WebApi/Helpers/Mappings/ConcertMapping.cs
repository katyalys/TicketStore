using AutoMapper;
using Catalog.Application.Dtos.ConcertDtos;
using Catalog.Domain.Entities;

namespace Catalog.WebApi.Helpers.Mappings
{
    public class ConcertMapping : Profile
    {
		public ConcertMapping()
		{
			CreateMap<Concert, ConcertsShortViewDto>()
					.ForMember(dest => dest.Place, opt => opt.MapFrom(src => src.Place))
					.ReverseMap();

			CreateMap<Concert, FullInfoConcertDto>()
					.ForMember(dest => dest.Place, opt => opt.MapFrom(src => src.Place))
					.ReverseMap();
		}
	}
}
