using Domain.Enums;
using Domain.Exceptions;
using Domain.Ports;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Guest
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string Email { get; set; }
        public PersonId DocumentId { get; set; } = null!;

        private void ValidateStatus()
        {
            if (
                DocumentId == null ||
                string.IsNullOrWhiteSpace(DocumentId.IdNumber) ||
                DocumentId.IdNumber.Length <= 3 ||
                !Enum.IsDefined(typeof(DocumentTypes), DocumentId.DocumentType)
            )
            {
                throw new InvalidPersonDucumentIdException();
            }

            if ( 
                string.IsNullOrWhiteSpace(Name) || 
                string.IsNullOrWhiteSpace(Surname) || 
                string.IsNullOrWhiteSpace(Email)
                )
            {
                throw new MissingRequiredInformation();
            }

            if (!Utils.ValidatEmail(Email))
            {
                throw new InvalidEmailException();
            }


        }

        public async Task Save(IGuestRepository guestRepository)
        {
            ValidateStatus();
            if (Id == 0)
            {
                Id = await guestRepository.Create(this);
            }
            else
            {
                // await guestRepository.Update(this);
            }
        }

    }
}
