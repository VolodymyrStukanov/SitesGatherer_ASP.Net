using SitesGatherer.Sevices.Serialization.ModelsDTO;
using SitesGatherer.Sevices.SitesStorageService;
using SitesGatherer.Sevices.SitesStorageService.Factories;

namespace SitesGatherer.Sevices.Serialization.Extensions
{
    public static class SerializationDataModelsExtensions
    {
        private static readonly IJsonSerializationService SerializationService = new JsonSerializationService();
        private static readonly SiteStorageFactory siteStorageFactory = new();
        public static string ToJson(this ISitesStorage storage)
        {
            var dto = storage.ToDto();
            return SerializationService.Serialize(dto);
        }
    
        public static ISitesStorage FromJson(this string json)
        {
            var dto = SerializationService.Deserialize<SitesStorageDto>(json);
            return dto != null ? siteStorageFactory.FromDto(dto) : new SitesStorage();
        }
    }
}