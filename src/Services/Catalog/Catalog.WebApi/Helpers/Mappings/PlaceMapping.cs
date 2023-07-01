using AutoMapper;
using Catalog.Application.Dtos.PlaceDtos;
using Catalog.Domain.Entities;

namespace Catalog.WebApi.Helpers.Mappings
{
    public class PlaceMapping : Profile
    {
        public PlaceMapping()
        {
            CreateMap<Place, PlaceDto>().ReverseMap();
            CreateMap<Place, OrderServerGrpc.PlaceDto>()
                .ForMember(dest => dest.PlaceNmuber, opt => opt.MapFrom(src => src.PlaceNumber))
                .ReverseMap();
        }
    }
}
