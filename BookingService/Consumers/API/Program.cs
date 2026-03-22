
using System.Text.Json.Serialization;
using Application.Payment.Ports;
using Data;
using Data.Booking;
using Data.Guest;
using Data.Room;
using Domain.Booking.Ports;
using Domain.Guest.Ports;
using Domain.Room.Ports;
using Microsoft.EntityFrameworkCore;
using Payment.Application;
using MediatR;
using Application.Booking.Queries;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<GetBookingQueryHandler>();
});

#region automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
#endregion

#region IoC
builder.Services.AddScoped<IGuestRepository, GuestRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepostory>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IPaymentProcessorFactory, PaymentProcessorFactory>();
#endregion

#region Database Context
var connectionString = builder.Configuration.GetConnectionString("Main");
builder.Services.AddDbContext<HotelDBContext>(options =>
    options.UseNpgsql(connectionString)
);
#endregion
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
#region OpenAPI Documentation
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();
#endregion

builder.Services.AddControllersWithViews()
    .AddJsonOptions(
        options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

