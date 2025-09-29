
using SitesGatherer.Sevices.Serialization.Interfaces;
using SitesGatherer.Sevices.Serialization.ModelsDTO;
using SitesGatherer.Sevices.ToLoadStorageService.Models;

namespace SitesGatherer.Sevices.ToLoadStorageService
{
    public interface IToLoadStorage : ISerializableData<ToLoadStorageDto>, IDeserializedStorage<ToLoadStorageDto>
    {
        public void AddToLoads(List<string> urls, string? parentDomain = null, int? parentshipDepth = null);
        public ToLoad? GetNext();
        public bool TryGetNext(out ToLoad? toLoad);
        public bool TryGetNextByDomain(out ToLoad toLoad, string domain);
        public int GetToLoadCount();
        public List<string> GetUniqueDomains();
        public void AddIgnoredDomain(string domain);
    }
}