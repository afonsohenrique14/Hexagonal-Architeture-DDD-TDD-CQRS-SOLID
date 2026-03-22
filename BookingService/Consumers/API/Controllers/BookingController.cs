
using Application;
using Application.Booking.Commands;
using Application.Booking.DTOs;
using Application.Booking.Queries;
using Application.Booking.Requests;
using Application.Payment.DTOs;
using Application.Payment.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class BookingController: ControllerBase
{

    private readonly ILogger<BookingController> _logger;

    private readonly IMediator _mediator;
    public BookingController(
        ILogger<BookingController> logger,
        IMediator mediator
    )
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<ReturnBookingDTO>> Post(CreateBookingDTO booking)
    {

        var request = new CreateBookingRequest { Data = booking };

        var command = new CreateBookingComand
        {
            createBookingRequest = request
        };

        
        var res = await _mediator.Send(command);

        // var res = await _bookingManager.CreateBooking(request);

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

        var command = new PayForABookingCommand
        {
            paymentRequestDTO = paymentRequestDTO
        };

        var res = await _mediator.Send(command);

        // var res = await _bookingManager.PayForABooking(paymentRequestDTO);

        if (res.Success) return Ok(res.Data);

        return BadRequest(res);

    }


    [HttpGet]
    public async Task<ActionResult<ReturnBookingDTO>> Get(int bookingId)
    {
        var query = new GetBookingQuery
        {
            bookingId = bookingId   
        };

        var res = await _mediator.Send(query);

        if(res.Success) return Ok(res.Data);

        if(res.ErrorCode == ErrorCodes.NOT_FOUND)
        {
            return NotFound(res);
        }

        _logger.LogError("Response with unknown ErrorCode Returned{@res}", res);
        return StatusCode(StatusCodes.Status500InternalServerError, res);
    }


}
