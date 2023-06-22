using AutoMapper;
using Catalog.Application.Dtos;
using Catalog.Application.Dtos.BasketDtos;
using Catalog.Application.Dtos.ConcertDtos;
using Catalog.Application.Dtos.PlaceDtos;
using Catalog.Application.Dtos.SectorDtos;
using Catalog.Application.Dtos.TicketDtos;
using Catalog.Domain;
using Catalog.Domain.Entities;
using Catalog.Domain.ErrorModels;
using System.Collections.Generic;

namespace Catalog.WebApi.Helpers
{
	public class MappingProfiles : Profile
	{
		public MappingProfiles()
		{
			//CreateMap<Concert, ConcertsShortViewDto>()
			//	.ForMember(dest => dest.Place, opt => opt.MapFrom(src => src.Place))
			//	.ReverseMap();

			//CreateMap<Place, PlaceDto>().ReverseMap();

			//CreateMap<Concert, FullInfoConcertDto>()
			//	.ForMember(dest => dest.Place, opt => opt.MapFrom(src => src.Place))
			//	.ReverseMap();

			//CreateMap(typeof(Result<>), typeof(Result<>));

			//CreateMap<Sector, SectorInfoDto>()
			//	.ForMember(dest => dest.SectorName, opt => opt.MapFrom(src => src.Name.ToString()))
			//	.ReverseMap();

			//CreateMap<Sector, SectorFullInffoDto>()
			//	.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.ToString()))
			//	.ReverseMap();

			//CreateMap<Basket, BasketDto>()
			//	.ReverseMap();

			//CreateMap<Ticket, TicketAddDto>()
			//	.ReverseMap();

			//CreateMap<Ticket, TicketDto>()
			//	.ForMember(dest => dest.Concert, opt => opt.MapFrom(src => src.Concert))
			//    .ForMember(dest => dest.SectorName, opt => opt.MapFrom(src => src.Sector.Name.ToString()))
			//	.ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Sector.Price))
			//	.ReverseMap();
		}
	}
}
