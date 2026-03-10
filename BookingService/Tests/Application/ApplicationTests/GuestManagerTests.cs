using Application;
using Application.Guest.DTOs;
using Application.Guest.Requests;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Ports;
using Domain.ValueObjects;
using Moq;

namespace ApplicationTests
{

    public class Tests
    {
        GuestManager _guestManager;
        int _createdGuestId = 111;

        [SetUp]
        public void Setup()
        {
            var fakeRepository = new Mock<IGuestRepository>();

            fakeRepository.Setup(
                x => x.Create(
                    It.IsAny<Guest>())
            ).Returns(
                Task.FromResult(_createdGuestId)
            );

            fakeRepository.Setup(x => x.Get(_createdGuestId))
              .ReturnsAsync(new Guest
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

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(
                x => x.Map<Guest>(It.IsAny<GuestDTO>())
            ).Returns( (GuestDTO dto) =>
                new Guest
                   { Name =dto.Name,
                    Surname = dto.Surname,
                    Email = dto.Email,
                    DocumentId = new PersonId
                    {
                        IdNumber = dto.IdNumber,
                        DocumentType = (DocumentTypes)dto.IdTypeCode
                    }
                }
                
            );

            mapperMock
            .Setup(x => x.Map<GuestDTO>(It.IsAny<Guest>()))
            .Returns((Guest g) => new GuestDTO
            {
                Id = g.Id,
                Name = g.Name,
                Surname = g.Surname,
                Email = g.Email,
                IdNumber = g.DocumentId.IdNumber,
                IdTypeCode = (int)g.DocumentId.DocumentType
            });


            _guestManager = new GuestManager(fakeRepository.Object, mapperMock.Object);
        }
        

        #region TESTES POSITIVOS
        [Test]
        public async Task ShouldCreateGuest()
        {
            var guestDTo = new GuestDTO
            {
                Name = "John",
                Surname = "Doe",
                Email = "john.doe@example.com",
                IdNumber = "123456789",
                IdTypeCode = 1
            };

            var createGuestRequest = new CreateGuestRequest
            {
                Data = guestDTo
            };

            var result = await _guestManager.CreateGuest(createGuestRequest);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data.Id, Is.EqualTo(_createdGuestId));
        }

        [Test]
        public async Task ShouldGetGuest()
        {
            var result = await _guestManager.GetGuest(_createdGuestId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data.Id, Is.EqualTo(_createdGuestId));
        }
        #endregion

        #region TESTES NEGATIVOS

         [TestCase("Jhon", "Doe" , "123", 1, ErrorCodes.INVALID_PERSON_DOCUMENT, "john.doe@example.com")]
         [TestCase("Jhon", "Doe" , "", 1, ErrorCodes.INVALID_PERSON_DOCUMENT, "john.doe@example.com")]
         [TestCase("Jhon", "Doe" ,"45656565", 11, ErrorCodes.INVALID_PERSON_DOCUMENT, "john.doe@example.com")]
         [TestCase("Jhon", "Doe" ,"45656565", 1, ErrorCodes.INVALID_EMAIL, "b@b.com")]
         [TestCase("Jhon", "Doe" ,"45656565", 1, ErrorCodes.MISSING_REQUIRED_INFORMATION, "")]
         [TestCase("Jhon", "Doe" ,"45656565", 1, ErrorCodes.MISSING_REQUIRED_INFORMATION, null)]
         [TestCase("", "Doe" ,"45656565", 1, ErrorCodes.MISSING_REQUIRED_INFORMATION, null)]
         [TestCase("Jhon", " " ,"45656565", 1, ErrorCodes.MISSING_REQUIRED_INFORMATION, null)]
         [TestCase("Jhon", "Doe" ,"", 1, ErrorCodes.INVALID_PERSON_DOCUMENT, null)]
         [TestCase("Jhon", "Doe" ,null, 1, ErrorCodes.INVALID_PERSON_DOCUMENT, "jhon.doe@example.com")]
        public async Task ShouldNotCreateGuestWithInvalidData(
            string name, 
            string surname,
            string? idNumber, 
            int idTypeCode, 
            ErrorCodes expectedErrorCode, 
            string? email
        )
        {
            var guestDTo = new GuestDTO
            {
                Name = name,
                Surname = surname,
                Email = email!,
                IdNumber = idNumber!,
                IdTypeCode = idTypeCode
            };

            var createGuestRequest = new CreateGuestRequest
            {
                Data = guestDTo
            };

            var result = await _guestManager.CreateGuest(createGuestRequest);

            Assert.That(result.ErrorCode, Is.EqualTo(expectedErrorCode));
            Assert.That(result.Success, Is.False);
        }

        [TestCase]
        public async Task ShoulReturnNotFoundWhenTryingToGetNonExistingGuest()
        {
            var result = await _guestManager.GetGuest(-1);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo(ErrorCodes.NOT_FOUND));
        }

        #endregion

        
    }
}
