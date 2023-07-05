using Catalog.Application.Dtos;
using Catalog.Application.Dtos.ConcertDtos;
using Catalog.Application.Dtos.PlaceDtos;
using Catalog.Application.Dtos.SectorDtos;
using Catalog.Application.Dtos.TicketDtos;
using Catalog.Application.FluentValidation;
using Catalog.Application.Interfaces;
using Catalog.Application.MassTransit.Consumers;
using Catalog.Domain.Interfaces;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Repositories;
using Catalog.Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit.MultiBus;
using Microsoft.EntityFrameworkCore;
using Shared.EventBus.Messages.Constants;
using Shared.MassTransit.Filters;
using StackExchange.Redis;
using System.Reflection;
using MassTransit;

namespace Catalog.WebApi.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddOtherExtensions(this IServiceCollection services, string connectionString,
            string redisConnectionString, Assembly assembly, ConfigurationManager config)
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

            services.AddMassTransit(x => 
            {
                var assembly = Assembly.GetAssembly(typeof(TicketStatusConsumer));
                x.AddConsumers(assembly);

                string host = config["RabbitMQ:Host"];
                var virtualHost = config["RabbitMQ:VirtualHost"];
                var username = config["RabbitMQ:Username"];
                var password = config["RabbitMQ:Password"];

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(host, virtualHost, h =>
                    {
                        h.Username(username);
                        h.Password(password);
                    });

                    cfg.UseConsumeFilter(typeof(ConsumeLoggingFilter<>), context);
                    cfg.UsePublishFilter(typeof(PublishLoggingFilter<>), context);

                    cfg.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, x =>
                    {
                        //x.Bind<GetTicketStatusEvent>();
                        x.ConfigureConsumer<TicketStatusConsumer>(context);
                    });
                });
            });

            return services;
        }
    }
}
