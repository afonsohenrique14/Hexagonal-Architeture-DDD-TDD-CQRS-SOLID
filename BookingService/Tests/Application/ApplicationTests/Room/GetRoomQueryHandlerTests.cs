using Application;
using Application.Room.Mappings;
using Application.Room.Queries;
using AutoMapper;
using Domain.Guest.ValueObjects;
using Domain.Room.Enums;
using Domain.Room.Ports;
using Entities = Domain.Room.Entities;
using Moq;

namespace ApplicationTests.Room;

public class GetRoomQueryHandlerTests
{
    private Mock<IRoomRepository> _roomRepository = null!;
    private IMapper _mapper = null!;
    private GetRoomQueryHandler _handler = null!;
    private readonly int _createdRoomId = 111;

    [SetUp]
    public void Setup()
    {
        _roomRepository = new Mock<IRoomRepository>();

        _roomRepository
            .Setup(x => x.Get(_createdRoomId))
            .ReturnsAsync(new Entities.Room
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
            });

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CreateRoomProfile>();
            cfg.AddProfile<ReturnRoomProfille>();
        });

        _mapper = config.CreateMapper();

        _handler = new GetRoomQueryHandler(_roomRepository.Object, _mapper);
    }

    [Test]
    public async Task Should_Get_Room()
    {
        // Arrange
        var query = new GetRoomQuery { roomId = _createdRoomId };

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data.Id, Is.EqualTo(_createdRoomId));
    }

    [Test]
    public async Task Should_Return_Not_Found_When_Room_Does_Not_Exist()
    {
        // Arrange
        var query = new GetRoomQuery { roomId = -1 };

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo(ErrorCodes.NOT_FOUND));
    }
}