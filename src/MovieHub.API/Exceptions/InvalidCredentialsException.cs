namespace MovieHub.API.Exceptions;

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException(string message) : base(message)
    {
    }

    public InvalidCredentialsException() : base()
    {
    }

    public InvalidCredentialsException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}