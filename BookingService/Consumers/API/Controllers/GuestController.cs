using Application;
using Application.Guest.DTOs;
using Application.Guest.Requests;
using Application.Ports;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class GuestController: ControllerBase
{
  
    private readonly ILogger<GuestController> _logger;
    private readonly IGuestManager _guestManager;

    public GuestController(
        ILogger<GuestController> logger,
        IGuestManager guestManager
        )
    {
        _logger = logger;
        _guestManager = guestManager;
    }

    [HttpPost]
    public  async Task<ActionResult<ReturnGuestDTO>> Post(CreateGuestDTO guest)
    {
        var request = new CreateGuestRequest { Data = guest };
        
        var res = await _guestManager.CreateGuest(request);

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
        var res = await _guestManager.GetGuest(guestId);

        if(res.Success) return Ok(res.Data);

        if(res.ErrorCode == ErrorCodes.NOT_FOUND)
        {
            return NotFound(res);
        }

        _logger.LogError("Response with unknown ErrorCode Returned{@res}", res);
        return StatusCode(StatusCodes.Status500InternalServerError, res);

    }



}
