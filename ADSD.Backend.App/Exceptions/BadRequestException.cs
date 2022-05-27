using System.Runtime.Serialization;

namespace ADSD.Backend.App.Exceptions;

public class BadRequestException : HttpCodeException
{
    public BadRequestException()
    {
    }

    protected BadRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public BadRequestException(string message) : base(message)
    {
    }

    public BadRequestException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public override int StatusCode => 400;
}