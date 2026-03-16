using System;
using Application.Room.DTOs;

namespace Application.Room.Requests;

public class CreateRoomRequest
{
     public CreateRoomDTO Data {get; set;}= null!;
}
