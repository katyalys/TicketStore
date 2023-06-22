using AutoMapper;
using Catalog.Domain.ErrorModels;

namespace Catalog.WebApi.Helpers.Mappings
{
    public class ResultMapping : Profile
    {
        public ResultMapping()
        {
            CreateMap(typeof(Result<>), typeof(Result<>));
        }
    }
}
