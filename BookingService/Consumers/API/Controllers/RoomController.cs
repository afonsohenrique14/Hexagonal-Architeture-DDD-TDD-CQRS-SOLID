
using Application;
using Application.Room.Commands;
using Application.Room.DTOs;
using Application.Room.Queries;
using Application.Room.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class RoomController: ControllerBase
{

    private readonly ILogger<RoomController> _logger;
    private readonly IMediator _mediator;

    public RoomController(
        ILogger<RoomController> logger,
        IMediator mediator
    )
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<ReturnRoomDTO>> Post(CreateRoomDTO room)
    {

        var request = new CreateRoomRequest { Data = room };
        var command = new CreateRoomCommand
        {
            createRoomRequest = request
        };

        var res = await _mediator.Send(command);

        // var res = await _roomManager.CreateRoom(request);

        if (res.Success) return Created("", res.Data);

        if (res.ErrorCode == ErrorCodes.ROOM_COULD_NOT_STORE_DATA)
        {
            return BadRequest(res);
        }

        _logger.LogError("Response with unknown ErrorCode Returned{@res}", res);
        return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");

    }

    [HttpGet]
        public async Task<ActionResult<ReturnRoomDTO>> Get(int roomId)
    {
        var query = new GetRoomQuery
        {
            roomId = roomId
        };

        var res = await _mediator.Send(query);
        // var res = await _roomManager.GetRoom(roomId);

        if(res.Success) return Ok(res.Data);

        if(res.ErrorCode == ErrorCodes.NOT_FOUND_ROOM)
        {
            return NotFound(res);
        }

        _logger.LogError("Response with unknown ErrorCode Returned{@res}", res);
        return StatusCode(StatusCodes.Status500InternalServerError, res);

    }

}
