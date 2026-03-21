
using Application;
using Application.Booking.DTOs;
using Application.Booking.Ports;
using Application.Booking.Requests;
using Application.Payment.DTOs;
using Application.Payment.Responses;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class BookingController: ControllerBase
{

    private readonly ILogger<BookingController> _logger;
    private readonly IBookingManager _bookingManager;
    public BookingController(
        ILogger<BookingController> logger,
        IBookingManager bookingManager
    )
    {
        _logger = logger;
        _bookingManager = bookingManager;
    }

    [HttpPost]
    public async Task<ActionResult<ReturnBookingDTO>> Post(CreateBookingDTO booking)
    {
        var request = new CreateBookingRequest { Data = booking };
        
        var res = await _bookingManager.CreateBooking(request);

        if(res.Success) return Created("", res.Data);

        if(
            res.ErrorCode == ErrorCodes.INVALID_DATES || 
            res.ErrorCode == ErrorCodes.MISSING_REQUIRED_INFORMATION || 
            res.ErrorCode == ErrorCodes.INVALID_GUEST_ID ||
            res.ErrorCode == ErrorCodes.INVALID_ROOM_ID ||
            res.ErrorCode == ErrorCodes.COULD_NOT_STORE_DATA ||
            res.ErrorCode == ErrorCodes.NOT_FOUND
        )
        {
            return BadRequest(res);
        }

        _logger.LogError("Response with unknown ErrorCode Returned{@res}", res);
        return StatusCode(StatusCodes.Status500InternalServerError, res);
    }

    [HttpPost]
    [Route("{bookingId}/Pay")]
    public async Task<ActionResult<PaymentResponse>> Pay(
        PaymentRequestDTO paymentRequestDTO, int bookingId
    )
    {
        paymentRequestDTO.BookingId = bookingId;

        var res = await _bookingManager.PayForABooking(paymentRequestDTO);

        if (res.Success) return Ok(res.Data);

        return BadRequest(res);

    }


    [HttpGet]
    public async Task<ActionResult<ReturnBookingDTO>> Get(int bookingId)
    {
        var res = await _bookingManager.GetBooking(bookingId);

        if(res.Success) return Ok(res.Data);

        if(res.ErrorCode == ErrorCodes.NOT_FOUND)
        {
            return NotFound(res);
        }

        _logger.LogError("Response with unknown ErrorCode Returned{@res}", res);
        return StatusCode(StatusCodes.Status500InternalServerError, res);
    }


}
