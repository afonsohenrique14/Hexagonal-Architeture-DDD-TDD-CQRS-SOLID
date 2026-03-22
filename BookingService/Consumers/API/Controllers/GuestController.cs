using Application;
using Application.Guest.Commands;
using Application.Guest.DTOs;
using Application.Guest.Queries;
using Application.Guest.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class GuestController: ControllerBase
{
  
    private readonly ILogger<GuestController> _logger;

    private readonly IMediator _mediator;

    public GuestController(
        ILogger<GuestController> logger,
        IMediator mediator
        )
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpPost]
    public  async Task<ActionResult<ReturnGuestDTO>> Post(CreateGuestDTO guest)
    {
        var request = new CreateGuestRequest { Data = guest };

        var command = new CreateGuestCommand
        {
            createGuestRequest = request
        };

        var res = await _mediator.Send(command);
        
        // var res = await _guestManager.CreateGuest(request);

        if(res.Success) return Created("", res.Data);

        if(
            res.ErrorCode == ErrorCodes.INVALID_PERSON_DOCUMENT || 
            res.ErrorCode == ErrorCodes.MISSING_REQUIRED_INFORMATION || 
            res.ErrorCode == ErrorCodes.INVALID_EMAIL ||
            res.ErrorCode == ErrorCodes.COULD_NOT_STORE_DATA ||
            res.ErrorCode == ErrorCodes.NOT_FOUND
        )
        {
            return BadRequest(res);
        }

        _logger.LogError("Response with unknown ErrorCode Returned{@res}", res);
        return StatusCode(StatusCodes.Status500InternalServerError, res);

    }

    [HttpGet]
    public async Task<ActionResult<ReturnGuestDTO>> Get(int guestId)
    {   
        var query = new GetGuestQuery
        {
            GuestId = guestId
        };

        var res = await _mediator.Send(query);
        // var res = await _guestManager.GetGuest(guestId);

        if(res.Success) return Ok(res.Data);

        if(res.ErrorCode == ErrorCodes.NOT_FOUND)
        {
            return NotFound(res);
        }

        _logger.LogError("Response with unknown ErrorCode Returned{@res}", res);
        return StatusCode(StatusCodes.Status500InternalServerError, res);

    }



}
