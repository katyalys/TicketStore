using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Identity.Application.Dtos;
using Identity.Domain;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Mapper
{
    public class AddMappingProfile : Profile
    {
        public AddMappingProfile()
        {
            CreateMap<IdentityUser, RegisterUser>().ReverseMap();
            CreateMap<IdentityUser, LoginUser>().ReverseMap();
            CreateMap<IdentityUser, UserViewModel>().ReverseMap();
        }
    }
}
