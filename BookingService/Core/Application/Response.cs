namespace Application;

public enum ErrorCodes
{
    NOT_FOUND = 1,
    COULD_NOT_STORE_DATA = 2,
    INVALID_PERSON_DOCUMENT = 3,
    MISSING_REQUIRED_INFORMATION = 4,
    INVALID_EMAIL = 5
}

public abstract class Response
{
    public bool Success {get; set;}
    public string Message { get; set; } = null!;
    public ErrorCodes ErrorCode {get; set;}
}
