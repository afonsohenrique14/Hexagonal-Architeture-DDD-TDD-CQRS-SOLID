
using Domain.Guest.Enums;

namespace Domain.Guest.ValueObjects;

public class PersonId
{
    public string IdNumber { get; set; } = null!;
    public DocumentTypes DocumentType { get; set; }
}
