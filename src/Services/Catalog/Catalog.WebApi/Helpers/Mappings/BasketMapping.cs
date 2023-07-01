using AutoMapper;
using Catalog.Application.Dtos.BasketDtos;
using Catalog.Domain.Entities;
using OrderServerGrpc;

namespace Catalog.WebApi.Helpers.Mappings
{
    public class BasketMapping : Profile
    {
        public BasketMapping()
        {
            CreateMap<Basket, BasketDto>()
               .ReverseMap();

            CreateMap<Basket, TicketOrderDto>()
               .ReverseMap();
        }
    }
}
