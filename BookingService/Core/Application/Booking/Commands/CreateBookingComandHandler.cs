using Application.Booking.DTOs;
using Application.Booking.Mappings;
using Application.Booking.Responses;
using AutoMapper;
using Domain.Booking.Ports;
using Domain.Guest.Ports;
using Domain.Room.Ports;
using MediatR;

namespace Application.Booking.Commands;

public class CreateBookingComandHandler : IRequestHandler<CreateBookingComand, BookingResponse>
{

    private readonly IBookingRepository _bookingRepository;
    private readonly IGuestRepository _guestRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IMapper _mapper;
    public CreateBookingComandHandler(IBookingRepository bookingRepository, IGuestRepository guestRepository, IRoomRepository roomRepository, IMapper mapper)
    {
        _bookingRepository = bookingRepository;
        _guestRepository = guestRepository;
        _roomRepository = roomRepository;
        _mapper = mapper;
    }
    public async Task<BookingResponse> Handle(CreateBookingComand request, CancellationToken cancellationToken)
    {
        try
        {
            var booking = _mapper.Map<Domain.Booking.Entities.Booking>(request.createBookingRequest.Data);

            await booking.Save(_bookingRepository
            , _guestRepository, _roomRepository);

            return ResponseFactory.Ok<BookingResponse>(r =>
            {
                r.Data = _mapper.Map<ReturnBookingDTO>(booking);
            });

        }
        catch (Exception ex)
        {
            var failure = BookingExceptionMapper.Map(ex);

            return ResponseFactory.Fail<BookingResponse>(failure);
        }
    }
}
