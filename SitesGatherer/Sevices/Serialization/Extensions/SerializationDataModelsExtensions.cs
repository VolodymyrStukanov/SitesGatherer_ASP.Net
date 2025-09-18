using SitesGatherer.Sevices.Serialization.ModelsDTO;
using SitesGatherer.Sevices.SitesStorageService;
using SitesGatherer.Sevices.SitesStorageService.Factories;
using SitesGatherer.Sevices.SitesStorageService.Interfaces;
using SitesGatherer.Sevices.ToLoadStorageService;

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
        public static string ToJson(this IToLoadStorage storage)
        {
            var dto = storage.ToDto();
            return SerializationService.Serialize(dto);
        }
    
        public static SitesStorage DataStorageFromJson(this string json)
        {
            var dto = SerializationService.Deserialize<SitesStorageDto>(json);
            return dto != null ? siteStorageFactory.FromDto(dto) : new SitesStorage();
        }
    
        public static ToLoadStorage ToLoadStorageFromJson(this string json, IParsedStorage parsedStorage, ISkippedStorage skippedStorage)
        {
            var dto = SerializationService.Deserialize<ToLoadStorageDto>(json);
            var storage = new ToLoadStorage(parsedStorage, skippedStorage);
            if (dto != null)
                storage.Restore(dto);
            return storage;
        }
    }
}