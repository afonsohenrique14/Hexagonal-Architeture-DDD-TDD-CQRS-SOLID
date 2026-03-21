using System;

namespace Application;

public sealed class FailureInfo
{
    public ErrorCodes ErrorCode { get; init; } = default!;
    public string Message { get; init; } = default!;

}