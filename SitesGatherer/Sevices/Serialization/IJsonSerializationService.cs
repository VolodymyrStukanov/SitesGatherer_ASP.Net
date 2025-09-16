
namespace SitesGatherer.Sevices.Serialization
{
    public interface IJsonSerializationService
    {
        string Serialize<T>(T obj);
        T? Deserialize<T>(string json);        
    }
}