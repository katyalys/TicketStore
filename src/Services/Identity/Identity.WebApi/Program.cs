using Events.Api.Extensions;
using Identity.Application.Mapper;
using Identity.Application.Services;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.Interfaces;
using Identity.WebApi.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var environment = builder.Environment;

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("ConnectionString");
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();
builder.Services.AddIdentityServerConfig(configuration, environment);
builder.Services.AddSwagger();
builder.Services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
builder.Services.AddScoped(typeof(AccountService));  //»«Ã≈Õ»“‹
builder.Services.AddAutoMapper(typeof(AddMappingProfile));
builder.Services.AddLocalApiAuthentication();

var app = builder.Build();

await app.UseDatabaseSeed();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
