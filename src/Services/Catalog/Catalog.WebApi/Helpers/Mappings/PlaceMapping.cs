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
        }
    }
}
