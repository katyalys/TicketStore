using Catalog.Application.Interfaces;
using Catalog.Domain.Interfaces;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Repositories;
using Catalog.Infrastructure.Services;
using Catalog.WebApi.Extensions;
using Catalog.WebApi.Helpers;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ConnectionString");
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<CatalogContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddAuthentificate();
builder.Services.AddSwagger();
builder.Services.AddAutoMapper(typeof(MappingProfiles));
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
builder.Services.AddScoped<IBasketService, BasketService>();
builder.Services.AddScoped<ISectorService, SectorService>();
builder.Services.AddScoped<ITicketService, TicketService>();

var app = builder.Build();

await app.UseDatabaseSeed();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
