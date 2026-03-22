using Application;
using Application.Guest.Commands;
using Application.Guest.DTOs;
using Application.Guest.Mappings;
using Application.Guest.Requests;
using AutoMapper;
using Domain.Guest.Ports;
using Moq;

namespace ApplicationTests.Guest;

public class CreateGuestCommandHandlerTests
{
    private Mock<IGuestRepository> _guestRepository = null!;
    private IMapper _mapper = null!;
    private CreateGuestCommandHandler _handler = null!;
    private readonly int _createdGuestId = 111;

    [SetUp]
    public void Setup()
    {
        _guestRepository = new Mock<IGuestRepository>();

        // Simula persistência: retorna ID e (importante) seta o ID na entidade que foi "salva".
        _guestRepository
            .Setup(x => x.Create(It.IsAny<Domain.Guest.Entities.Guest>()))
            .ReturnsAsync(_createdGuestId)
            .Callback<Domain.Guest.Entities.Guest>(g => g.Id = _createdGuestId);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CreateGuestProfile>();
            cfg.AddProfile<ReturnGuestProfile>();
        });

        _mapper = config.CreateMapper();

        _handler = new CreateGuestCommandHandler(_guestRepository.Object, _mapper);
    }

    [Test]
    public async Task Should_Create_Guest()
    {
        // Arrange
        var guestDto = new CreateGuestDTO
        {
            Name = "John",
            Surname = "Doe",
            Email = "john.doe@example.com",
            IdNumber = "123456789",
            IdTypeCode = 1
        };

        var createGuestRequest = new CreateGuestRequest { Data = guestDto };

        var command = new CreateGuestCommand
        {
            createGuestRequest = createGuestRequest
        };

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data.Id, Is.EqualTo(_createdGuestId));
    }

    [TestCase("Jhon", "Doe", "123", 1, ErrorCodes.INVALID_PERSON_DOCUMENT, "john.doe@example.com")]
    [TestCase("Jhon", "Doe", "", 1, ErrorCodes.INVALID_PERSON_DOCUMENT, "john.doe@example.com")]
    [TestCase("Jhon", "Doe", "45656565", 11, ErrorCodes.INVALID_PERSON_DOCUMENT, "john.doe@example.com")]
    [TestCase("Jhon", "Doe", "45656565", 1, ErrorCodes.INVALID_EMAIL, "b@b.com")]
    [TestCase("Jhon", "Doe", "45656565", 1, ErrorCodes.MISSING_REQUIRED_INFORMATION, "")]
    [TestCase("Jhon", "Doe", "45656565", 1, ErrorCodes.MISSING_REQUIRED_INFORMATION, null)]
    [TestCase("", "Doe", "45656565", 1, ErrorCodes.MISSING_REQUIRED_INFORMATION, null)]
    [TestCase("Jhon", " ", "45656565", 1, ErrorCodes.MISSING_REQUIRED_INFORMATION, null)]
    [TestCase("Jhon", "Doe", "", 1, ErrorCodes.INVALID_PERSON_DOCUMENT, null)]
    [TestCase("Jhon", "Doe", null, 1, ErrorCodes.INVALID_PERSON_DOCUMENT, "jhon.doe@example.com")]
    public async Task Should_Not_Create_Guest_With_Invalid_Data(
        string name,
        string surname,
        string? idNumber,
        int idTypeCode,
        ErrorCodes expectedErrorCode,
        string? email)
    {
        // Arrange
        var guestDto = new CreateGuestDTO
        {
            Name = name,
            Surname = surname,
            Email = email!,      // mantém seu padrão (mesmo podendo ser null)
            IdNumber = idNumber!,
            IdTypeCode = idTypeCode
        };

        var request = new CreateGuestRequest { Data = guestDto };

        var command = new CreateGuestCommand
        {
            createGuestRequest = request
        };

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo(expectedErrorCode));
    }
}