using System;
using Domain.Guest.Exceptions;

namespace Application.Guest.Mappings;

public static class GuestExceptionMapper
{
    public static FailureInfo Map(Exception ex)
        {
            return ex switch
            {
                
                InvalidPersonDucumentIdException => new FailureInfo
                {
                        ErrorCode = ErrorCodes.INVALID_PERSON_DOCUMENT,
                        Message = "The document id or document type provided is invalid"
                },
                MissingRequiredInformation => new FailureInfo
                {

                        ErrorCode = ErrorCodes.MISSING_REQUIRED_INFORMATION,
                        Message = "Missing required information"
                },
                InvalidEmailException => new FailureInfo
                {

                        ErrorCode = ErrorCodes.INVALID_EMAIL,
                        Message = "The email provided is invalid"
                },
                _ => new FailureInfo
                {

                        ErrorCode = ErrorCodes.COULD_NOT_STORE_DATA,
                        Message = "There was an erro when saving to DB"
                }
            };
        }
}
