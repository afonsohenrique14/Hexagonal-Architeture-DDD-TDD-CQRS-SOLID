namespace Application.Guest.DTOs;


public class CreateGuestDTO
{
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string Email { get; set; }

    public string IdNumber { get; set; } = null!;
    public int IdTypeCode { get; set; }

}
