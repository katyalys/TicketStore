using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Order.Infrastructure.Data;
using Order.WebApi.Extensions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ConnectionString");
var assembly = Assembly.GetExecutingAssembly();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<OrderContext>(options =>
           options.UseSqlServer(connectionString));
builder.Services.AddAuthentification();
builder.Services.AddSwagger();
builder.Services.AddAutoMapper(assembly);
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

app.UseAuthorization();

app.MapControllers();

app.Run();
