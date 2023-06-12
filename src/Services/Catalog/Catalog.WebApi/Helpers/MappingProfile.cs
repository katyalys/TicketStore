using AutoMapper;
using Catalog.Application.Dtos;
using Catalog.Application.Dtos.SectorDtos;
using Catalog.Domain;
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

			CreateMap<Sector, SectorInfoDto>()
				.ForMember(dest => dest.SectorName, opt => opt.MapFrom(src => src.Name.ToString()))
				.ReverseMap();

			CreateMap<Sector, SectorFullInffoDto>()
				.ForMember(dest => dest.Price, opt => opt.MapFrom(src => 
											   TicketPriceCalculator.CalculatePrice(src.Price, src.Name, src.Place.City)))
				.ReverseMap();
		}

	}
}
