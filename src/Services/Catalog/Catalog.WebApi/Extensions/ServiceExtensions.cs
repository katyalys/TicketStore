using Catalog.Application.Dtos;
using Catalog.Application.Dtos.ConcertDtos;
using Catalog.Application.Dtos.PlaceDtos;
using Catalog.Application.Dtos.SectorDtos;
using Catalog.Application.Dtos.TicketDtos;
using Catalog.Application.FluentValidation;
using Catalog.Application.Interfaces;
using Catalog.Domain.Interfaces;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Repositories;
using Catalog.Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Reflection;

namespace Catalog.WebApi.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddOtherExtensions(this IServiceCollection services, string connectionString,
            string redisConnectionString, Assembly assembly)
        {
            services.AddControllers();
            services.AddFluentValidationAutoValidation();
            services.AddControllers();
            services.AddFluentValidationAutoValidation();
            services.AddScoped<IValidator<FullInfoConcertDto>, FullInfoConcertDtoValidator>();
            services.AddScoped<IValidator<PlaceDto>, PlaceDtoValidator>();
            services.AddScoped<IValidator<SectorFullInffoDto>, SectorFullInfoValidator>();
            services.AddScoped<IValidator<TicketAddDto>, TicketAddDtoValidator>();
            services.AddScoped<AuthUserDto>();
            services.AddDbContext<CatalogContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddAutoMapper(assembly);
            services.AddSingleton<IConnectionMultiplexer>(c =>
            {
                var configuration = ConfigurationOptions.Parse(redisConnectionString, true);
                return ConnectionMultiplexer.Connect(configuration);
            });
            services.AddScoped<IRedisRepository, RedisRepository>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICatalogService, CatalogService>();
            services.AddScoped<IPlaceService, PlaceService>();
            services.AddScoped<IBackgroundJobsService, BackgroundJobsService>();
            services.AddScoped<IBasketService, BasketService>();
            services.AddScoped<ISectorService, SectorService>();
            services.AddScoped<ITicketService, TicketService>();

            services.AddGrpc(opt =>
            {
                opt.EnableDetailedErrors = true;
            });

            return services;
        }
    }
}
