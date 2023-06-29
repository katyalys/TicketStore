using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Order.Domain.Interfaces;
using Order.Infrastructure.Data;
using Order.Infrastructure.Repositories;
using Order.WebApi.Extensions;
using System.Reflection;
using Order.Application;
using OrderClientGrpc;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ConnectionString");
var assembly = Assembly.GetExecutingAssembly();

HttpClient myHttpClient = new HttpClient
{
    DefaultRequestVersion = HttpVersion.Version20,
    DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower
};

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly(), Order.Application.AssemblyReference.Assembly));
builder.Services.AddDbContext<OrderContext>(options =>
           options.UseSqlServer(connectionString));
builder.Services.AddAuthentification();
builder.Services.AddSwagger();
builder.Services.AddAutoMapper(assembly);
builder.Services.AddGrpcClient<OrderProtoService.OrderProtoServiceClient>(o =>
{
    o.Address = new Uri("http://localhost:5046");
});
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.UseDatabaseSeed();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
