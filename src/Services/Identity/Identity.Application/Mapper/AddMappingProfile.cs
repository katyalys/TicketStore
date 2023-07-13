using AutoMapper;
using Identity.Application.Dtos;
using IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using Shared.EventBus.Messages.Events;

namespace Identity.Application.Mapper
{
    public class AddMappingProfile : Profile
    {
        public AddMappingProfile()
        {
            CreateMap<IdentityUser, RegisterUser>().ReverseMap();
            CreateMap<IdentityUser, LoginUser>().ReverseMap();
            CreateMap<IdentityUser, UserViewModel>().ReverseMap();
            CreateMap<TokenResponse, TokenViewModel>()
                .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(src => src.HttpStatusCode));
            CreateMap<IdentityUser, DeleteOrdersEvent>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ReverseMap();
        }
    }
}
