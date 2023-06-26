using AutoMapper;
using Catalog.Application.Dtos.BasketDtos;
using Catalog.Domain.Entities;

namespace Catalog.WebApi.Helpers.Mappings
{
    public class BasketMapping : Profile
    {
        public BasketMapping()
        {
            CreateMap<Basket, BasketDto>()
                .ReverseMap();
        }
    }
}
