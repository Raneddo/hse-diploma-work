using System.Runtime.Serialization;

namespace ADSD.Backend.App.Exceptions;

public abstract class HttpCodeException : Exception
{
    protected HttpCodeException()
    {
    }

    protected HttpCodeException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    protected HttpCodeException(string message) : base(message)
    {
    }

    protected HttpCodeException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public abstract int StatusCode { get; }
}