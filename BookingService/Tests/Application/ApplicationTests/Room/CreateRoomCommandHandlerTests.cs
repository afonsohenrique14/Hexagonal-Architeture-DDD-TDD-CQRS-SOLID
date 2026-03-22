using Application;
using Application.Room.Commands;
using Application.Room.DTOs;
using Application.Room.Mappings;
using Application.Room.Requests;
using AutoMapper;
using Domain.Room.Enums;
using Domain.Room.Ports;
using Entities = Domain.Room.Entities;
using Moq;


namespace ApplicationTests.Room;

public class CreateRoomCommandHandlerTests
{
    private Mock<IRoomRepository> _roomRepository = null!;
    private IMapper _mapper = null!;
    private CreateRoomCommandHandler _handler = null!;
    private readonly int _createdRoomId = 111;

    [SetUp]
    public void Setup()
    {
        _roomRepository = new Mock<IRoomRepository>();

        // Simula persistência: retorna ID e garante que a entidade "salva" recebe o Id.
        _roomRepository
            .Setup(x => x.Create(It.IsAny<Entities.Room>()))
            .ReturnsAsync(_createdRoomId)
            .Callback<Entities.Room>(room => room.Id = _createdRoomId);

        // AutoMapper real (profiles mínimos)
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CreateRoomProfile>();
            cfg.AddProfile<ReturnRoomProfille>();
        });
        _mapper = config.CreateMapper();

        _handler = new CreateRoomCommandHandler(_roomRepository.Object, _mapper);
    }

    [Test]
    public async Task Should_Return_Created_Room_Id()
    {
        // Arrange
        var roomDto = new CreateRoomDTO
        {
            Name = "Room 101",
            Level = 15,
            InMaintenance = false,
            Value = 100,
            Currency = (int)AcceptedCurrencies.Dollar
        };

        var request = new CreateRoomRequest { Data = roomDto };

        var command = new CreateRoomCommand
        {
            createRoomRequest = request
        };

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data.Id, Is.EqualTo(_createdRoomId));
    }

    [TestCase(null, 15, false, 100, 1, ErrorCodes.MISSING_REQUIRED_INFORMATION_ROOM)]  // Nome inválido
    [TestCase("John", 15, false, -100, 1, ErrorCodes.INVALID_ROOM_PRICE)]              // Preço inválido
    [TestCase("John", 15, false, 100, -15, ErrorCodes.INVALID_ROOM_PRICE)]            // Moeda inválida (conforme seu domínio)
    [TestCase("John", -15, false, 100, 1, ErrorCodes.INVALID_ROOM_LEVEL)]             // Andar inválido
    public async Task Should_Return_Error_When_Creating_Room_With_Invalid_Data(
        string? name,
        int level,
        bool inMaintenance,
        decimal value,
        int currency,
        ErrorCodes expectedErrorCode)
    {
        // Arrange
        var roomDto = new CreateRoomDTO
        {
            Name = name!,           // você já usa ! no teste original
            Level = level,
            InMaintenance = inMaintenance,
            Value = value,
            Currency = currency
        };

        var request = new CreateRoomRequest { Data = roomDto };

        var command = new CreateRoomCommand
        {
            createRoomRequest = request
        };

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo(expectedErrorCode));
    }
}