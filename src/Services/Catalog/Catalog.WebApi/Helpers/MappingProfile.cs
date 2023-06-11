using AutoMapper;
using Catalog.Application.Dtos;
using Catalog.Domain.Entities;

namespace Catalog.WebApi.Helpers
{
	public class MappingProfiles : Profile
	{
		public MappingProfiles()
		{
			CreateMap<Concert, ConcertsShortViewModel>()
				.ForMember(dest => dest.Place, opt => opt.MapFrom(src => src.Place))
				.ReverseMap();

			CreateMap<Place, PlaceModel>().ReverseMap();

			CreateMap<Concert, FullInfoConcertModel>()
				.ForMember(dest => dest.Place, opt => opt.MapFrom(src => src.Place))
				.ReverseMap();
		}

	}
}
