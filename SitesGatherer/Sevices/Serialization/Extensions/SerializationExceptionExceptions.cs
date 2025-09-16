
namespace SitesGatherer.Sevices.Serialization.Extensions
{
    public class SerializationExceptionExceptions : Exception
    {
        public SerializationExceptionExceptions(string message) : base(message) { }
        public SerializationExceptionExceptions(string message, Exception innerException) : base(message, innerException) { }
    }
}