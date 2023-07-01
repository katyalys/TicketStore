using Catalog.Domain.Interfaces;
using Catalog.WebApi.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Hangfire;
using Catalog.Infrastructure.BackgroundJobs;
using Catalog.Infrastructure.Services;
using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

//builder.WebHost.ConfigureKestrel(options =>
//{
//    options.Listen(IPAddress.Any, 5046, listenOptions =>
//    {
//        listenOptions.Protocols = HttpProtocols.Http2;
//    });
//});

var connectionString = builder.Configuration.GetConnectionString("ConnectionString");
var connectionHangfireString = builder.Configuration.GetConnectionString("HangfireConnectionString");
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
var assembly = Assembly.GetExecutingAssembly();

builder.Services.AddHangfire(connectionHangfireString);
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
