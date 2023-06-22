using AutoMapper;
using Catalog.Application.Dtos.SectorDtos;
using Catalog.Domain.Entities;

namespace Catalog.WebApi.Helpers.Mappings
{
    public class SectorMapping : Profile
    {
        public SectorMapping()
        {
            CreateMap<Sector, SectorInfoDto>()
                .ForMember(dest => dest.SectorName, opt => opt.MapFrom(src => src.Name.ToString()))
                .ReverseMap();

            CreateMap<Sector, SectorFullInffoDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.ToString()))
                .ReverseMap();
        }
    }
}
