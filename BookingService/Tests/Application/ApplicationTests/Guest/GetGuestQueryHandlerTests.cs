using Application;
using Application.Guest.Mappings;
using Application.Guest.Queries;
using AutoMapper;
using Domain.Guest.Enums;
using Domain.Guest.Ports;
using Domain.Guest.ValueObjects;
using Moq;

namespace ApplicationTests.Guest;

public class GetGuestQueryHandlerTests
{
    private Mock<IGuestRepository> _guestRepository = null!;
    private IMapper _mapper = null!;
    private GetGuestQueryHandler _handler = null!;
    private readonly int _createdGuestId = 111;

    [SetUp]
    public void Setup()
    {
        _guestRepository = new Mock<IGuestRepository>();

        _guestRepository
            .Setup(x => x.Get(_createdGuestId))
            .ReturnsAsync(new Domain.Guest.Entities.Guest
            {
                Id = _createdGuestId,
                Name = "John",
                Surname = "Doe",
                Email = "john.doe@example.com",
                DocumentId = new PersonId
                {
                    IdNumber = "123456789",
                    DocumentType = DocumentTypes.DriverLicense
                }
            });

        // Para qualquer id não configurado explicitamente, Moq retorna null (default),
        // então isso já cobre o caso NotFound sem setup adicional.

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CreateGuestProfile>();
            cfg.AddProfile<ReturnGuestProfile>();
        });

        _mapper = config.CreateMapper();

        _handler = new GetGuestQueryHandler(_guestRepository.Object, _mapper);
    }

    [Test]
    public async Task Should_Get_Guest()
    {
        // Arrange
        var query = new GetGuestQuery { GuestId = _createdGuestId };

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data.Id, Is.EqualTo(_createdGuestId));
    }

    [Test]
    public async Task Should_Return_Not_Found_When_Trying_To_Get_Non_Existing_Guest()
    {
        // Arrange
        var query = new GetGuestQuery { GuestId = -1 };

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo(ErrorCodes.NOT_FOUND));
    }
}