using Microsoft.EntityFrameworkCore;
using Order.Domain.Interfaces;
using Order.Infrastructure.Data;
using Order.Infrastructure.Repositories;
using Order.WebApi.Extensions;
using System.Reflection;
using FluentValidation.AspNetCore;
using OrderClientGrpc;
using System.Net;
using FluentValidation;
using Order.Application.Features.Orders.Commands.CancelOrder;
using Order.Application.FluentValidation;
using Order.Application.Features.Orders.Commands.CancelTicket;
using Order.Application.Features.Orders.Commands.CheckoutOrder;
using Order.Application.Features.Orders.Queries.TicketDetailedInfo;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("ConnectionString");
var assembly = Assembly.GetExecutingAssembly();

HttpClient myHttpClient = new HttpClient
{
    DefaultRequestVersion = HttpVersion.Version20,
    DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower
};

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddScoped<IValidator<CancelOrderCommand>, CancelOrderValidator>();
builder.Services.AddScoped<IValidator<CancelTicketCommand>, CancelTicketValidator>();
builder.Services.AddScoped<IValidator<CheckoutOrderCommand>, CheckoutOrderValidator>();
builder.Services.AddScoped<IValidator<TicketsDetailedQuery>, TicketDetailedValidator>();
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
builder.Services.AddMassTransitConfig(configuration);

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
