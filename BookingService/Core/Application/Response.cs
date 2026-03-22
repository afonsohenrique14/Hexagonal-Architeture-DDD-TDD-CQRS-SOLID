namespace Application;

public enum ErrorCodes
{

    #region  GUEST ERROR
    NOT_FOUND = 1,
    COULD_NOT_STORE_DATA = 2,
    INVALID_PERSON_DOCUMENT = 3,
    MISSING_REQUIRED_INFORMATION = 4,
    INVALID_EMAIL = 5,
    #endregion

    #region ROOM ERROR
    NOT_FOUND_ROOM = 101,
    ROOM_COULD_NOT_STORE_DATA = 102,

    INVALID_ROOM_PRICE = 103,

    MISSING_REQUIRED_INFORMATION_ROOM = 104,

    INVALID_ROOM_LEVEL = 105,
    #endregion
    # region BOOKING ERROR

    BOOKING_COULD_NOT_BE_CREATED = 201,
    INVALID_DATES = 202,

    INVALID_GUEST_ID = 203,
    INVALID_ROOM_ID = 204,

    MISSING_REQUIRED_INFORMATION_BOOKING = 205,

    INVALID_DATA_GUEST = 206,
    INVALID_DATA_ROOM = 207,

    INAVAILABLE_ROOM = 208,

    CONFLICTING_BOOKING = 209,
    #endregion
    # region PAYMENT ERROR
    INVALID_PAYMENT_INTENTION = 301,

    PAYMENT_PROVIDER_NOT_IMPLEMENTED = 302,

    INVALID_BOOKING_ID = 303,
    DUPLICATED_PAYMENT = 304,
    #endregion
}

public abstract class Response
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public ErrorCodes ErrorCode { get; set; }
}
