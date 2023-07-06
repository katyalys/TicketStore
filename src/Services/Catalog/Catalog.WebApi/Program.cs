using Catalog.Domain.Interfaces;
using Catalog.WebApi.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Hangfire;
using Catalog.Infrastructure.BackgroundJobs;
using Catalog.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("ConnectionString");
var connectionHangfireString = configuration.GetConnectionString("HangfireConnectionString");
var redisConnectionString = configuration.GetConnectionString("Redis");
var assembly = Assembly.GetExecutingAssembly();

builder.Services.AddHangfire(connectionHangfireString);
builder.Services.AddMassTransitConfig(configuration);
builder.Services.AddAuthentification();
builder.Services.AddSwagger();
builder.Services.AddOtherExtensions(connectionString, redisConnectionString, assembly);

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var redisRepository = serviceProvider.GetRequiredService<IRedisRepository>();
    redisRepository.ExpiredKeyNotification();
}

await app.UseDatabaseSeed();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard("/mydashboard");
app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<GrpcOrderService>();
app.MapControllers();

HangfireUpdateConcert.HangfireUpdateConcerts();
app.Run();
