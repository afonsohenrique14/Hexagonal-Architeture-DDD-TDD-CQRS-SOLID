namespace Domain.Booking.Enums;

public enum Action
{
    Pay,
    Finish, // after paid and used 
    Cancel, // can never be paid
    Refound, // Paind then refound
    Reopen // canceled

}
