using Newtonsoft.Json;
using SitesGatherer.Sevices.Serialization.Extensions;

namespace SitesGatherer.Sevices.Serialization
{
    public class JsonSerializationService : IJsonSerializationService
    {
        private readonly JsonSerializerSettings settings;
    
        public JsonSerializationService()
        {
            settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,

                // Handle circular references if any
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                // Preserve type information if needed
                TypeNameHandling = TypeNameHandling.None
            };
        }

        public T? Deserialize<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json, settings);
            }
            catch (Exception ex)
            {
                throw new SerializationExceptionExceptions($"Failed to deserialize to {typeof(T).Name}", ex);
            }
        }

        public string Serialize<T>(T obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj, settings);
            }
            catch (Exception ex)
            {
                throw new SerializationExceptionExceptions($"Failed to serialize {typeof(T).Name}", ex);
            }
        }
    }
}