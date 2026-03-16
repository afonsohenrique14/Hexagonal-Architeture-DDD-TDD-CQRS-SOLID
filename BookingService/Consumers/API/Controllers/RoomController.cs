
using Application;
using Application.Room.DTOs;
using Application.Room.Ports;
using Application.Room.Requests;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class RoomController: ControllerBase
{

    private readonly ILogger<RoomController> _logger;
    private readonly IRoomManager _roomManager;

    public RoomController(
        ILogger<RoomController> logger,
        IRoomManager roomManager
    )
    {
        _logger = logger;
        _roomManager = roomManager;
    }

    [HttpPost]
    public async Task<ActionResult<ReturnRoomDTO>> Post(CreateRoomDTO room)
    {
        var request = new CreateRoomRequest { Data = room };

        var res = await _roomManager.CreateRoom(request);

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
        var res = await _roomManager.GetRoom(roomId);

        if(res.Success) return Ok(res.Data);

        if(res.ErrorCode == ErrorCodes.NOT_FOUND_ROOM)
        {
            return NotFound(res);
        }

        _logger.LogError("Response with unknown ErrorCode Returned{@res}", res);
        return StatusCode(StatusCodes.Status500InternalServerError, res);

    }

}
