
using Application.Booking.DTOs;
using Application.Booking.Responses;
using AutoMapper;
using Domain.Booking.Ports;
using MediatR;

namespace Application.Booking.Queries;

public class GetBookingQueryHandler : IRequestHandler<GetBookingQuery, BookingResponse>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IMapper _mapper;

    public GetBookingQueryHandler(
        IBookingRepository bookingRepository,
        IMapper mapper
        )
    {
        _bookingRepository = bookingRepository;
        _mapper = mapper;
    }
    public async Task<BookingResponse> Handle(GetBookingQuery request, CancellationToken cancellationToken)
    {
        var guest = await _bookingRepository.Get(request.bookingId);

        if (guest == null)
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
