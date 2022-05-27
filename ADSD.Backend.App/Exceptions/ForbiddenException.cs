using System.Runtime.Serialization;

namespace ADSD.Backend.App.Exceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException()
    {
    }

    protected ForbiddenException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ForbiddenException(string message) : base(message)
    {
    }

    public ForbiddenException(string message, Exception innerException) : base(message, innerException)
    {
    }
}