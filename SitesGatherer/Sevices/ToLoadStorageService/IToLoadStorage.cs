
using SitesGatherer.Sevices.ToLoadStorageService.Models;

namespace SitesGatherer.Sevices.ToLoadStorageService
{
    public interface IToLoadStorage
    {
        public IEnumerable<ToLoad> AddToLoads(List<string> urls, string? parentDomain = null, int? parentshipDepth = null);
        public ToLoad? GetNext();
        public bool TryGetNext(out ToLoad? toLoad);
        public int GetToLoadCount();
    }
}