using System;

namespace Application.Room.DTOs;

public class ReturnRoomDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Level { get; set; }
    public bool InMaintenance { get; set; }
    public decimal Value { get; set; }
    public int Currency { get; set; }
    // public bool IsAvailable {get;}
    // public bool HasGuest {get;}
}
