namespace MovieHub.API.Exceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message)
    {
    }
    public ForbiddenException() : base()
    {
    }

    public ForbiddenException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}