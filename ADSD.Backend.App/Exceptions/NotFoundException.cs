using System.Runtime.Serialization;

namespace ADSD.Backend.App.Exceptions;

public class NotFoundException : HttpCodeException
{
    public NotFoundException()
    {
    }

    public NotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public override int StatusCode => 404;
}