using Events.Api.Extensions;
using FluentValidation;
using Identity.Application.FluentValidation;
using Identity.Application.Interfaces;
using Identity.Application.Mapper;
using Identity.Application.Services;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.Interfaces;
using Identity.WebApi.Extensions;
using IdentityServer;
using Microsoft.EntityFrameworkCore;
using FluentValidation.AspNetCore;
using Identity.Application.Dtos;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var environment = builder.Environment;

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("ConnectionString");
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddScoped<IValidator<RegisterUser>, RegisterUserValidator>();
builder.Services.AddScoped<IValidator<LoginUser>, LoginUserValidator>();
builder.Services.AddIdentityServerConfig(configuration, environment);
builder.Services.AddSwagger();
builder.Services.AddScoped(typeof(IUserAccessService), typeof(UserAccessService));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, IdentityTokenService>();
builder.Services.AddScoped(typeof(IdentityTokenService));
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
