using Application;
using Application.Booking;
using Application.Booking.DTOs;
using Application.Booking.Mappings;
using Application.Booking.Requests;
using Application.Guest.Mappings;
using Application.Room.Mappings;
using AutoMapper;
using Domain.Booking.Entities;
using Domain.Booking.Ports;
using Domain.Guest.Enums;
using Domain.Guest.Entities;
using Domain.Guest.Ports;
using Domain.Room.Entities;
using Domain.Room.Ports;
using Domain.Room.Enums;
using Domain.Guest.ValueObjects;
using Moq;

namespace ApplicationTests;

public class BookingManagerTests
{
    private BookingManager _bookingManager = null!;
    private Mock<IBookingRepository> _bookingRepo = null!;
    private Mock<IRoomRepository> _roomRepo = null!;
    private Mock<IGuestRepository> _guestRepo = null!;
    private IMapper _mapper = null!;
    private readonly int _createdBookingId = 111;
    private readonly int _inAvailableRoomId = 121;
    private readonly int _invalidGuestId = 999;
    private readonly int _invalidRoomId = 999;

    private List<Booking> _store = null!;

    [SetUp]
    public void Setup()
    {
        _bookingRepo = new Mock<IBookingRepository>();
        _roomRepo = new Mock<IRoomRepository>();
        _guestRepo = new Mock<IGuestRepository>();

        
        _store = new List<Booking>();  // ✅ ESSENCIAL


        _bookingRepo
            .Setup(x => x.Create(It.IsAny<Booking>()))
            .ReturnsAsync(_createdBookingId)
            .Callback<Booking>(booking =>
            {
                // Simula a "persistência" em um store in-memory
                _store.Add(new Booking
                {
                    RoomId = booking.RoomId,
                    GuestId = booking.GuestId,
                    Start = booking.Start,
                    End = booking.End,
                    PlacedAt = booking.PlacedAt
                });

            });

        //  Importante: se seu ReturnBookingDTO inclui Room/Guest (DTOs),
        // então aqui simulamos o repositório real retornando com navegações preenchidas
        _bookingRepo
            .Setup(x => x.Get(_createdBookingId))
            .ReturnsAsync(new Booking
            {
                Id = _createdBookingId,
                PlacedAt = DateTime.UtcNow,
                Start = DateTime.UtcNow.AddDays(1),
                End = DateTime.UtcNow.AddDays(2),
                RoomId = 1,
                GuestId = 1,
                Room = new Room
                {
                    Id = 1,
                    Name = "Room 101",
                    Level = 15,
                    InMaintenance = false,
                    Price = new Price { Value = 100, Currency = AcceptedCurrencies.Dollar }
                },
                Guest = new Guest
                {
                    Id = 1,
                    Name = "John",
                    Surname = "Doe",
                    Email = "john.doe@example.com",
                    DocumentId = new PersonId { IdNumber = "123456789", DocumentType = DocumentTypes.DriverLicense }
                }
            });

        

        //  Garantir que Room/Guest “existem” para a validação no domínio (Validate)
        _roomRepo.Setup(x => x.Get(1)).ReturnsAsync(
            new Room 
            { 
                Id = 1,   
                Name = "Room 101", 
                Level = 15,
                InMaintenance = false,
                Price = new Price 
                    { 
                        Value = 100, 
                        Currency = AcceptedCurrencies.Dollar 
                    }
            }
            );
        _guestRepo.Setup(x => x.Get(1)).ReturnsAsync(
            new Guest 
            { 
                Id = 1, 
                Name = "John", 
                Surname = "Doe", 
                Email = "john.doe@example.com",
                DocumentId = new PersonId 
                    { 
                        IdNumber = "123456789", 
                        DocumentType = DocumentTypes.DriverLicense 
                    }
                }
            );

        _bookingRepo
        .Setup(x => x.ExistsActiveBookingForRoom(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
        .ReturnsAsync((int roomId, DateTime start, DateTime end) =>
            _store.Any(b =>
                b.RoomId == roomId &&
                b.Start < end &&
                b.End > start
            )
        );

        _roomRepo.Setup(x => x.Get(_invalidRoomId)).ReturnsAsync(new Room { Id = _invalidRoomId,   Name = "Room 101", }); // Simulando dados inválidos, não apenas inexistentes
        _guestRepo.Setup(x => x.Get(_invalidGuestId)).ReturnsAsync(new Guest { Id = _invalidGuestId, Name = " ", Surname = "Doe", Email = "john.doe@example.com" }); // Simulando dados inválidos, não apenas inexistentes

        _roomRepo.Setup(x => x.Get(_inAvailableRoomId)).ReturnsAsync(new Room
        {
            Id = _inAvailableRoomId,
            Name = "Room 121",
            InMaintenance = true, //  torna IsAvailable == false
        });

        //  AutoMapper real (mínimo de profiles)
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CreateBookingProfile>();
            cfg.AddProfile<ReturnBookingProfile>();
            cfg.AddProfile<CreateRoomProfile>();
            cfg.AddProfile<ReturnRoomProfille>();
            cfg.AddProfile<CreateGuestProfile>();
            cfg.AddProfile<ReturnGuestProfile>();
        });

        //  config.AssertConfigurationIsValid();
        _mapper = config.CreateMapper();

        _bookingManager = new BookingManager(
            _bookingRepo.Object,
            _guestRepo.Object,
            _roomRepo.Object,
            _mapper
        );
    }

    #region TESTES POSITIVOS
    [Test]
    public async Task CreateBooking_Should_Return_Created_Booking()
    {
        // Arrange
        var createBookingDto = new CreateBookingDTO
        {
            Start = DateTime.UtcNow.AddDays(1),
            End = DateTime.UtcNow.AddDays(3),
            RoomId = 1,
            GuestId = 1
        };

        var request = new CreateBookingRequest
        {
            Data = createBookingDto
        };

        // Act
        var result = await _bookingManager.CreateBooking(request);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Success, Is.True);
        Assert.That(_createdBookingId, Is.EqualTo(result.Data.Id));
    }

    [Test]
    public async Task ShouldGetBookingById()
    {
        // Act
        var result = await _bookingManager.GetBooking(_createdBookingId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data.Id, Is.EqualTo(_createdBookingId));
        Assert.That(result.Data.Room, Is.Not.Null);
        Assert.That(result.Data.Guest, Is.Not.Null);
    }


    #endregion
    #region TESTES NEGATIVOS
    [TestCase("2024-01-10", "2024-01-05", 1, 1, ErrorCodes.INVALID_DATES)] // End antes do Start
    [TestCase("2024-01-10", "2024-01-10", 1, 1, ErrorCodes.INVALID_DATES)] // End igual ao Start
    [TestCase("2024-01-10", "2024-01-15", 995, 1, ErrorCodes.INVALID_ROOM_ID)] // RoomId inexistente
    [TestCase("2024-01-10", "2024-01-15", 1, 995, ErrorCodes.INVALID_GUEST_ID)] // GuestId inexistente
    [TestCase("2024-01-10", "2024-01-15", 0, 1, ErrorCodes.MISSING_REQUIRED_INFORMATION_BOOKING)] // RoomId não fornecido
    [TestCase("2024-01-10", "2024-01-15", 1, 0, ErrorCodes.MISSING_REQUIRED_INFORMATION_BOOKING)] // GuestId não fornecido
    [TestCase("2024-01-10", "2024-01-15", 121, 1, ErrorCodes.INVALID_DATA_ROOM)] // RoomId de um quarto indisponível
    [TestCase("2024-01-10", "2024-01-15", 999, 1, ErrorCodes.INVALID_DATA_ROOM)] // RoomId inválido(simulando dados inválidos, não apenas inexistentes)
    [TestCase("2024-01-10", "2024-01-15", 1, 999, ErrorCodes.INVALID_DATA_GUEST)] // GuestId inválido(simulando dados inválidos, não apenas inexistentes)
    public async Task CreateBooking_Should_Return_Error_For_Invalid_Datas(
        DateTime start,
        DateTime end,
        int roomId,
        int guestId,
        ErrorCodes expectedErrorCode
        )
    {
        // Arrange
        var createBookingDto = new CreateBookingDTO
        {
            Start = start,
            End = end,
            RoomId = roomId,
            GuestId = guestId
        };

        var request = new CreateBookingRequest
        {
            Data = createBookingDto
        };

        // Act
        var result = await _bookingManager.CreateBooking(request);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo(expectedErrorCode));
    }

    [TestCase("2024-01-10", "2024-01-15")] // Datas iguais à primeira reserva criada 
    [TestCase("2024-01-09", "2024-01-11")] // Sobreposição no início
    [TestCase("2024-01-14", "2024-01-16")] // Sobreposição no fim
    [TestCase("2024-01-09", "2024-01-16")] // Sobreposição total (Contido completamente dentro da primeira reserva)
    [TestCase("2024-01-08", "2024-01-16")] // Sobreposição total (Contendo completamente a primeira reserva)
    public async Task CreateBooking_Should_Return_Conflict_When_Second_Booking_Overlaps( DateTime start, DateTime end)
    {
        // Arrange: primeira reserva (vai "persistir" em _store via Callback)
        var first = new CreateBookingDTO
        {
            Start = new DateTime(2024, 1, 10),
            End   = new DateTime(2024, 1, 15),
            RoomId = 1,
            GuestId = 1
        };

        var firstRes = await _bookingManager.CreateBooking(new CreateBookingRequest { Data = first });
        Assert.That(firstRes.Success, Is.True);

        // Act: segunda reserva com sobreposição parcial (12-14 dentro de 10-15)
        var second = new CreateBookingDTO
        {
            Start = start,
            End   = end,
            RoomId = 1,
            GuestId = 1
        };

        var secondRes = await _bookingManager.CreateBooking(new CreateBookingRequest { Data = second });

        // Assert
        Assert.That(secondRes.Success, Is.False);
        Assert.That(secondRes.ErrorCode, Is.EqualTo(ErrorCodes.CONFLICTING_BOOKING));

        // (opcional) garante que só a primeira foi persistida
        Assert.That(_store.Count, Is.EqualTo(1));
    }
    #endregion
}