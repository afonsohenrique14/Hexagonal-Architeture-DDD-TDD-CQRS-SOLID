using Application.Booking.Commands;
using Application.Booking.DTOs;
using Application.Booking.Mappings;
using Application.Booking.Ports;
using Application.Booking.Requests;
using Application.Booking.Responses;
using Application.Payment.DTOs;
using Application.Payment.Ports;
using Application.Payment.Responses;
using AutoMapper;
using Domain.Booking.Ports;
using Domain.Guest.Ports;
using Domain.Room.Ports;

namespace Application.Booking;

public class BookingManager : IBookingManager
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IGuestRepository _guestRepository;
    private readonly IRoomRepository _roomRepository;

    private readonly IPaymentProcessorFactory _paymentProcessorFactory;


    private readonly IMapper _mapper;

    public BookingManager(IBookingRepository bookingRepository, IGuestRepository guestRepository, IRoomRepository roomRepository, IMapper mapper, IPaymentProcessorFactory paymentProcessorFactory)
    {
        _bookingRepository = bookingRepository;
        _guestRepository = guestRepository;
        _roomRepository = roomRepository;
        _mapper = mapper;
        _paymentProcessorFactory = paymentProcessorFactory;
    }

    public async Task<BookingResponse> CreateBooking(CreateBookingRequest request)
    {
        try
        {
            var booking = _mapper.Map<Domain.Booking.Entities.Booking>(request.Data);

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

    public async Task<BookingResponse> GetBooking(int bookingId)
    {
        var guest = await _bookingRepository.Get(bookingId);

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

    public async Task<PaymentResponse> PayForABooking(PaymentRequestDTO paymentRequestDTO)
    {
        try
        {
            var booking = await _bookingRepository.Get(paymentRequestDTO.BookingId);

            if (booking == null)
            {
                return new PaymentResponse
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_BOOKING_ID,
                    Message = "No booking found with the provided id"
                };
            }

            if (booking.CurrentStatus != Domain.Booking.Enums.Status.Created)
            {
                return new PaymentResponse
                {
                    Success = false,
                    ErrorCode = ErrorCodes.DUPLICATED_PAYMENT,
                    Message = "The current booking was already paid for."
                };
            }

            var paymentProcessor = _paymentProcessorFactory.GetPaymentProcessor(paymentRequestDTO.SelectedPaymentProvider);

            var response = await paymentProcessor.CapturePayment(paymentRequestDTO.PaymentIntention);

            if (!response.Success)
            {
                return response;
            }

            booking.ChangeState(Domain.Booking.Enums.Action.Pay);

            await booking.Save(_bookingRepository, _guestRepository, _roomRepository);

            return ResponseFactory.Ok<PaymentResponse>(r =>
                {
                    r.Data = response.Data;
                    r.Message = "Payment successfully processed";
                }
            );
        }
        catch (Exception ex)
        {
            var failure = BookingExceptionMapper.Map(ex);

            return ResponseFactory.Fail<PaymentResponse>(failure);
        }

    }

}
