
using Domain.Booking.Enums;
using R = Domain.Room.Entities;
using Domain.Booking.Ports;
using Domain.Guest.Ports;
using Domain.Booking.Exceptions;
using Domain.Room.Ports;
using G = Domain.Guest.Entities;
using Action = Domain.Booking.Enums.Action;

namespace Domain.Booking.Entities;

public class Booking
{
    public int Id { get; set; }
    public DateTime PlacedAt { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    private Status Status { get; set; }
    public R.Room Room { get; set; } = null!;
    public int RoomId { get; set; }
    public G.Guest Guest { get; set; } = null!;
    public int GuestId { get; set; }
    public Status CurrentStatus => Status;

    public Booking()
    {
        Status = Status.Created;
        PlacedAt = DateTime.UtcNow;
    }

    public void ChangeState(Action action)
    {
        Status = (Status, action) switch
        {
            (Status.Created,    Action.Pay)     => Status.Paid,
            (Status.Created,    Action.Cancel)  => Status.Canceled,
            (Status.Paid,       Action.Finish)  => Status.Finished,
            (Status.Paid,       Action.Refound) => Status.Refounded,
            (Status.Canceled,   Action.Reopen)  => Status.Created,
            _ => Status

        };
    }

    private async Task Validate(IGuestRepository guestRepository, IRoomRepository roomRepository, IBookingRepository bookingRepository)
    {
        if(
            PlacedAt == default || 
            Start == default || 
            End == default ||
            GuestId == default ||
            RoomId == default
        )
        {
            throw new MissingRequiredInformation();
        }

        if (Start >= End)
        {
            throw new InvalidBookingDatesException();
        }

        var guest = await guestRepository.Get(GuestId);
        if (guest == null)
        {
            throw new InvalidGuestIDException();
        }

        var room =  await roomRepository.Get(RoomId);
        if (room == null)
        {
            throw new InvalidRoomIDException();
        }

        var hasConflictingBooking = await bookingRepository.ExistsActiveBookingForRoom(Id, RoomId, Start, End);
        if (hasConflictingBooking){
            throw new ConflictingBookingException();
        }

        guest.Isvalid();
        room.CanBeBooked();

    }

    public async Task Save(IBookingRepository bookingRepository,
     IGuestRepository guestRepository, IRoomRepository roomRepository
     ){
        await Validate(guestRepository, roomRepository, bookingRepository);
        
        if (Id == 0)
        {
            Id = await bookingRepository.Create(this);
        }
        else
        {
            await bookingRepository.Update(this);
        }
    }

}
