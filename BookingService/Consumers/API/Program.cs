using Application;
using Application.Booking;
using Application.Booking.Ports;
using Application.Ports;
using Application.Room;
using Application.Room.Ports;
using Data;
using Data.Booking;
using Data.Guest;
using Data.Room;
using Domain.Booking.Ports;
using Domain.Guest.Ports;
using Domain.Room.Ports;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

#region automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
#endregion

#region IoC
builder.Services.AddScoped<IGuestManager, GuestManager>();
builder.Services.AddScoped<IGuestRepository, GuestRepository>();
builder.Services.AddScoped<IRoomManager, RoomManager>();
builder.Services.AddScoped<IRoomRepository, RoomRepostory>();
builder.Services.AddScoped<IBookingManager, BookingManager>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

