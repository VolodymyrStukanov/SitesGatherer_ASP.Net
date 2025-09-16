using SitesGatherer.Sevices.HTMLParser.Models;
using SitesGatherer.Sevices.Serialization.Interfaces;
using SitesGatherer.Sevices.Serialization.ModelsDTO;
using SitesGatherer.Sevices.SitesStorageService.Models;
using SitesGatherer.Sevices.ToLoadStorageService.Models;

namespace SitesGatherer.Sevices.SitesStorageService
{
    public interface ISitesStorage : ISerializableData<SitesStorageDto>, IDeserializedStorage<IEnumerable<Site>>
    {
        public void StorePage(ParsedPage parsedPage, ToLoad toLoad);

        public bool LimitReached(string domain);

        public Dictionary<string, string> GetSitesData();
        public bool Contains(string domain, string[] pathParts);
        public void ResetIterator();
        public Page? GetNextPage();
    }
}