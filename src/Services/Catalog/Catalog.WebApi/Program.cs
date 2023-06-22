using Catalog.Application.Interfaces;
using Catalog.Domain.Interfaces;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Repositories;
using Catalog.Infrastructure.Services;
using Catalog.WebApi.Extensions;
using Catalog.WebApi.Helpers;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using FluentValidation.AspNetCore;
using FluentValidation;
using Catalog.Application.FluentValidation;
using Catalog.Application.Dtos;
using Catalog.Application.Dtos.SectorDtos;
using Catalog.Application.Dtos.TicketDtos;
using Catalog.Application.Dtos.PlaceDtos;
using Catalog.Application.Dtos.ConcertDtos;
using System.Reflection;
using System.Security.Principal;
using Microsoft.Web.Infrastructure;
using Hangfire;
using Hangfire.SqlServer;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ConnectionString");
var connectionHangfireString = builder.Configuration.GetConnectionString("HangfireConnectionString");
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
var assembly = Assembly.GetExecutingAssembly(); 

builder.Services.AddControllers();
//builder.Services.AddHangfire(x => x.UseSqlServerStorage(connectionHangfireString));
builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(connectionHangfireString, new SqlServerStorageOptions
        {
            PrepareSchemaIfNecessary = true,
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true
        })); ;
builder.Services.AddHangfireServer();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddScoped<IValidator<FullInfoConcertModel>, FullInfoConcertModelValidator>();
builder.Services.AddScoped<IValidator<PlaceModel>, PlaceModelValidator>();
builder.Services.AddControllers(); 
 builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddScoped<IValidator<FullInfoConcertDto>, FullInfoConcertDtoValidator>();
builder.Services.AddScoped<IValidator<PlaceDto>, PlaceDtoValidator>();
builder.Services.AddScoped<IValidator<SectorFullInffoDto>, SectorFullInfoValidator>();
builder.Services.AddScoped<IValidator<TicketAddDto>, TicketAddDtoValidator>();
builder.Services.AddScoped<AuthUserDto>();
builder.Services.AddDbContext<CatalogContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddAuthentificate();
builder.Services.AddSwagger();
builder.Services.AddAutoMapper(assembly);
builder.Services.AddSingleton<IConnectionMultiplexer>(c =>
{
    var configuration = ConfigurationOptions.Parse(redisConnectionString, true);
    return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddScoped<IRedisRepository, RedisRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<IPlaceService, PlaceService>();
builder.Services.AddScoped<IBackgroundJobsService, BackgroundJobsService>();
builder.Services.AddScoped<IBasketService, BasketService>();
builder.Services.AddScoped<ISectorService, SectorService>();
builder.Services.AddScoped<ITicketService, TicketService>();

var app = builder.Build();

await app.UseDatabaseSeed();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard("/mydashboard");
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
