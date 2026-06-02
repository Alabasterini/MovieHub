using System;
namespace MovieHub.Client.Exceptions;

public class PasswordMissMatchException : Exception
{
    public PasswordMissMatchException(string message) : base(message)
    {
    }

    public PasswordMissMatchException() : base()
    {
    }

    public PasswordMissMatchException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
