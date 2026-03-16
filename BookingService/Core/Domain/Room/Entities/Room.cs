using Domain.Room.Exceptions;
using Domain.Room.Ports;
using Domain.Guest.ValueObjects;
using Entities_booking = Domain.Booking.Entities;
using Domain.Booking.Enums;

namespace Domain.Room.Entities;

public class Room
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Level { get; set; }
    public bool InMaintenance { get; set; }
    public Price Price { get; set; } = null!;

    public ICollection<Entities_booking.Booking> Bookings { get; set; } = null!;

    public bool CanBeBooked()
    {
        ValdateStatus();
        return IsAvailable;
    }
    private bool IsAvailable { 
        get
        {
            if (InMaintenance)
            {
                return false;
            }
            return true;
        }
    }
    
    private void ValdateStatus()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new InvalidRoomDataException();
        }

        if (
            Price == null || 
            Price.Value <= 0 ||
            !Enum.IsDefined(typeof(Enums.AcceptedCurrencies), Price.Currency)
        )
        {
            throw new InvalidRoomPriceException();
        }

        if(Level < 0)
        {
            throw new InvalidRoomLevelException();
        }

        

    }

    public async Task Save(IRoomRepository roomRepository)
    {   
        ValdateStatus();
        if (Id == 0)
        {
            Id = await roomRepository.Create(this);
        }
        else
        {
            // await roomRepository.Update(this);
        }
    }
}
