using System.Runtime.Serialization;

namespace ADSD.Backend.App.Exceptions;

public class InvalidUserException : ArgumentException
{
    public InvalidUserException()
    {
    }

    protected InvalidUserException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public InvalidUserException(string message) : base(message)
    {
    }

    public InvalidUserException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public InvalidUserException(string message, string paramName) : base(message, paramName)
    {
    }

    public InvalidUserException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
    {
    }
}