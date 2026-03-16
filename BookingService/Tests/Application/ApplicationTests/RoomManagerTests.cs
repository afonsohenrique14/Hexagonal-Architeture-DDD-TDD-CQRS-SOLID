
using Application;
using Application.Room;
using Application.Room.DTOs;
using Application.Room.Mappings;
using Application.Room.Requests;
using AutoMapper;
using Entities = Domain.Room.Entities;
using Domain.Room.Ports;
using Moq;
using Domain.Room.Enums;
using Domain.Guest.ValueObjects;

namespace ApplicationTests;

public class RoomManagerTests
{
    RoomManager _roomManager;
    int _createdRoomId = 111;

    [SetUp]
    public void Setup()
    {
        
        var fakeRepository = new Mock<IRoomRepository>();

        fakeRepository.Setup(
            x => x.Create( 
                It.IsAny<Entities.Room>())
        ).Returns(
            Task.FromResult(_createdRoomId)
        );

        fakeRepository.Setup(
            x => x.Get(_createdRoomId)
        ).ReturnsAsync(
            new Entities.Room
            {
                Id = _createdRoomId,
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
    
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CreateRoomProfile>();
            cfg.AddProfile<ReturnRoomProfille>();
        });
    
        var mapper = config.CreateMapper();
        _roomManager = new RoomManager(fakeRepository.Object, mapper);
    }

    #region TESTES POSITIVOS

    [Test]
    public async Task Should_Return_Created_RoomId()
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

        var createRoomRequest = new CreateRoomRequest
        {
            Data = roomDto
        };

        // Act
        var result = await _roomManager.CreateRoom(createRoomRequest);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Data.Id, Is.EqualTo(_createdRoomId));
        Assert.That(result.Success, Is.True);

    }

    [Test]
    public async Task ShouldGetRoom()
    {
        var result = await _roomManager.GetRoom(_createdRoomId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data.Id, Is.EqualTo(_createdRoomId));
    }

    #endregion

    #region TESTES NEGATIVOS
    [TestCase(null, 15, false, 100, 1, ErrorCodes.MISSING_REQUIRED_INFORMATION_ROOM )]  // Invalid name
    [TestCase("Jhon", 15, false, -100, 1, ErrorCodes.INVALID_ROOM_PRICE )]  // Invalid price
    [TestCase("Jhon", 15, false, 100, -15, ErrorCodes.INVALID_ROOM_PRICE )] // Invalid currency
    [TestCase("Jhon", -15, false, 100, 1, ErrorCodes.INVALID_ROOM_LEVEL )] // Invalid level
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
            Name = name!, // Invalid name
            Level = level,
            InMaintenance = inMaintenance,
            Value = value,
            Currency = currency 
        };

        var createRoomRequest = new CreateRoomRequest
        {
            Data = roomDto
        };

        // Act
        var result = await _roomManager.CreateRoom(createRoomRequest);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo(expectedErrorCode));
    }
    #endregion
}
