using Application.Booking.Mappings;
using Application.Guest.Mappings;
using Application.Room.Mappings;
using AutoMapper;
using Domain.Booking.Ports;
using Domain.Guest.Enums;
using Domain.Guest.Ports;
using Domain.Guest.ValueObjects;
using Domain.Room.Enums;
using Domain.Room.Ports;
using Moq;
using Application.Payment.Ports;
using Application.Payment.Enums;
using Application.Payment.DTOs;
using Application.Payment.Responses;
using Application.Payment;
using Entities_booking = Domain.Booking.Entities;
using Entities_Guest = Domain.Guest.Entities;
using Entities_Room = Domain.Room.Entities;

namespace ApplicationTests.Booking;

public abstract class BookingTestFixtureBase
{
    protected Mock<IBookingRepository> BookingRepo = null!;
    protected Mock<IRoomRepository> RoomRepo = null!;
    protected Mock<IGuestRepository> GuestRepo = null!;
    protected Mock<IPaymentProcessorFactory> PaymentProcessorFactory = null!;
    protected Mock<IPaymentProcessor> PaymentProcessor = null!;
    protected IMapper Mapper = null!;

    protected readonly int CreatedBookingId = 111;
    protected readonly int InAvailableRoomId = 121;
    protected readonly int InvalidGuestId = 999;
    protected readonly int InvalidRoomId = 999;

    protected List<Entities_booking.Booking> Store = null!;

    [SetUp]
    public void BaseSetup()
    {
        BookingRepo = new Mock<IBookingRepository>();
        RoomRepo = new Mock<IRoomRepository>();
        GuestRepo = new Mock<IGuestRepository>();
        PaymentProcessorFactory = new Mock<IPaymentProcessorFactory>();
        PaymentProcessor = new Mock<IPaymentProcessor>();

        Store = new List<Entities_booking.Booking>();

        // Create => gera ID e "persiste" no store
        BookingRepo
            .Setup(x => x.Create(It.IsAny<Entities_booking.Booking>()))
            .ReturnsAsync(CreatedBookingId)
            .Callback<Entities_booking.Booking>(booking =>
            {
                booking.Id = CreatedBookingId; // importante p/ testes com Id

                Store.Add(new Entities_booking.Booking
                {
                    Id = booking.Id,
                    RoomId = booking.RoomId,
                    GuestId = booking.GuestId,
                    Start = booking.Start,
                    End = booking.End,
                    PlacedAt = booking.PlacedAt
                });
            });

        // Get => booking com navegações preenchidas
        BookingRepo
            .Setup(x => x.Get(CreatedBookingId))
            .ReturnsAsync(new Entities_booking.Booking
            {
                Id = CreatedBookingId,
                PlacedAt = DateTime.UtcNow,
                Start = DateTime.UtcNow.AddDays(1),
                End = DateTime.UtcNow.AddDays(2),
                RoomId = 1,
                GuestId = 1,
                Room = new Entities_Room.Room
                {
                    Id = 1,
                    Name = "Room 101",
                    Level = 15,
                    InMaintenance = false,
                    Price = new Price { Value = 100, Currency = AcceptedCurrencies.Dollar }
                },
                Guest = new Entities_Guest.Guest
                {
                    Id = 1,
                    Name = "John",
                    Surname = "Doe",
                    Email = "john.doe@example.com",
                    DocumentId = new PersonId { IdNumber = "123456789", DocumentType = DocumentTypes.DriverLicense }
                }
            });

        // Room/Guest válidos
        RoomRepo.Setup(x => x.Get(1)).ReturnsAsync(new Entities_Room.Room
        {
            Id = 1,
            Name = "Room 101",
            Level = 15,
            InMaintenance = false,
            Price = new Price { Value = 100, Currency = AcceptedCurrencies.Dollar }
        });

        GuestRepo.Setup(x => x.Get(1)).ReturnsAsync(new Entities_Guest.Guest
        {
            Id = 1,
            Name = "John",
            Surname = "Doe",
            Email = "john.doe@example.com",
            DocumentId = new PersonId { IdNumber = "123456789", DocumentType = DocumentTypes.DriverLicense }
        });

        // Conflito: não considera o mesmo Id
        BookingRepo
            .Setup(x => x.ExistsActiveBookingForRoom(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync((int bookingId, int roomId, DateTime start, DateTime end) =>
                Store.Any(b =>
                    b.Id != bookingId &&
                    b.RoomId == roomId &&
                    b.Start < end &&
                    b.End > start
                )
            );

        // Room/Guest inválidos (dados inválidos)
        RoomRepo.Setup(x => x.Get(InvalidRoomId)).ReturnsAsync(new Entities_Room.Room { Id = InvalidRoomId, Name = "Room 101" });
        GuestRepo.Setup(x => x.Get(InvalidGuestId)).ReturnsAsync(new Entities_Guest.Guest { Id = InvalidGuestId, Name = " ", Surname = "Doe", Email = "john.doe@example.com" });

        // Room indisponível
        RoomRepo.Setup(x => x.Get(InAvailableRoomId)).ReturnsAsync(new Entities_Room.Room
        {
            Id = InAvailableRoomId,
            Name = "Room 121",
            InMaintenance = true
        });

        // AutoMapper real
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CreateBookingProfile>();
            cfg.AddProfile<ReturnBookingProfile>();
            cfg.AddProfile<CreateRoomProfile>();
            cfg.AddProfile<ReturnRoomProfille>();
            cfg.AddProfile<CreateGuestProfile>();
            cfg.AddProfile<ReturnGuestProfile>();
        });

        Mapper = config.CreateMapper();

        // Payment
        var response = new PaymentResponse
        {
            Success = true,
            Message = "Payment sucessfully processed",
            Data = new PaymentStateDTO
            {
                CreatedDate = DateTime.Now,
                Message = "Sucesssfully paid",
                PaymentId = "123",
                Status = Status.Success
            }
        };

        PaymentProcessor
            .Setup(x => x.CapturePayment(It.IsAny<string>()))
            .ReturnsAsync(response);

        PaymentProcessorFactory
            .Setup(x => x.GetPaymentProcessor(It.IsAny<SupportedPaymentProviders>()))
            .Returns(PaymentProcessor.Object);
    }
}