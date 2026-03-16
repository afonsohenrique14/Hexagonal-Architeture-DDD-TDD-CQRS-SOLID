using System;
using Application.Booking.DTOs;
using Application.Booking.Ports;
using Application.Booking.Requests;
using Application.Booking.Responses;
using AutoMapper;
using Domain.Booking.Exceptions;
using Domain.Booking.Ports;
using Domain.Guest.Ports;
using Domain.Room.Ports;

namespace Application.Booking;

public class BookingManager : IBookingManager
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IGuestRepository _guestRepository;
    private readonly IRoomRepository _roomRepository;


    private readonly IMapper _mapper;

    public BookingManager(IBookingRepository bookingRepository, IGuestRepository guestRepository, IRoomRepository roomRepository, IMapper mapper)
    {
        _bookingRepository = bookingRepository;
        _guestRepository = guestRepository;
        _roomRepository = roomRepository;
        _mapper = mapper;
    }
    
    public async Task<BookingResponse> CreateBooking(CreateBookingRequest request)
    {   
        try{     

            var booking = _mapper.Map<Domain.Booking.Entities.Booking>(request.Data);

            await booking.Save(_bookingRepository
            , _guestRepository, _roomRepository);

            return new BookingResponse
            {
                Data = _mapper.Map<ReturnBookingDTO>(booking),
                Success = true,
            };
        }
        catch (MissingRequiredInformation)
        {
            return new BookingResponse
            {
                Success = false,
                ErrorCode = ErrorCodes.MISSING_REQUIRED_INFORMATION_BOOKING,
                Message = "Some required information for creating the booking was not provided"
            };
        }
        catch (InvalidBookingDatesException)
        {
            return new BookingResponse
            {
                Success = false,
                ErrorCode = ErrorCodes.INVALID_DATES,
                Message = "The provided booking dates are invalid"
            };
        }
        catch (ConflictingBookingException)
        {
            return new BookingResponse
            {
                Success = false,
                ErrorCode = ErrorCodes.CONFLICTING_BOOKING,
                Message = "The provided room is not available for the selected dates"
            };
        }
        catch (Domain.Guest.Exceptions.GuestExceptons)
        {
            return new BookingResponse
            {
                Success = false,    
                ErrorCode = ErrorCodes.INVALID_DATA_GUEST,
                Message = "The provided guest has invalid data"
            };
        }
        catch (Domain.Room.Exceptions.RoomExceptions)
        {
            return new BookingResponse
            {
                Success = false,
                ErrorCode = ErrorCodes.INVALID_DATA_ROOM,
                Message = "The provided room has invalid data"
            };
        }
        catch (InvalidGuestIDException)
        {
            return new BookingResponse
            {
                Success = false,
                ErrorCode = ErrorCodes.INVALID_GUEST_ID,
                Message = "The provided guest has invalid data"
            };
        }
        catch (InvalidRoomIDException)
        {
            return new BookingResponse
            {
                Success = false,
                ErrorCode = ErrorCodes.INVALID_ROOM_ID,
                Message = "The provided room does not exist"
            };
        }
        catch (Exception)
        {
            return new BookingResponse
            {
                Success = false,
                ErrorCode = ErrorCodes.BOOKING_COULD_NOT_BE_CREATED,
                Message = "An error occurred while creating the booking"
            };
        }
    }

    public async Task<BookingResponse> GetBooking(int bookingId)
    {
        var guest = await _bookingRepository.Get(bookingId);

        if(guest == null)
        {
            return new BookingResponse
            {
                Success = false,
                ErrorCode = ErrorCodes.NOT_FOUND,
                Message = "No booking found with the provided id"
            };
        }

        return new BookingResponse
        {
            Data = _mapper.Map<ReturnBookingDTO>(guest),
            Success = true,
        };

    }
}
