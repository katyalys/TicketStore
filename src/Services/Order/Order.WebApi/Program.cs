using Microsoft.EntityFrameworkCore;
using Order.Domain.Interfaces;
using Order.Infrastructure.Data;
using Order.Infrastructure.Repositories;
using Order.WebApi.Extensions;
using System.Reflection;
using OrderClientGrpc;
using System.Net;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
LoggingExtension.ConfigureLogging();
builder.Host.UseSerilog();

var connectionString = builder.Configuration.GetConnectionString("ConnectionString");
var assembly = Assembly.GetExecutingAssembly();

HttpClient myHttpClient = new HttpClient
{
    DefaultRequestVersion = HttpVersion.Version20,
    DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower
};

builder.Services.AddControllers();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly(), Order.Application.AssemblyReference.Assembly));
builder.Services.AddDbContext<OrderContext>(options =>
           options.UseSqlServer(connectionString));
builder.Services.AddAuthentification();
builder.Services.AddValidation();
builder.Services.AddSwagger();
builder.Services.AddAutoMapper(assembly);
builder.Services.AddGrpcClient<OrderProtoService.OrderProtoServiceClient>(o =>
{
    o.Address = new Uri(builder.Configuration["GrpcServer:Address"]);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.UseDatabaseSeed();
app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
