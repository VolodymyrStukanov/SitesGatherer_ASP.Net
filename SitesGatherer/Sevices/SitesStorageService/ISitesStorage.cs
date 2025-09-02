using SitesGatherer.Sevices.HTMLParser.Models;
using SitesGatherer.Sevices.SitesStorageService.Models;
using SitesGatherer.Sevices.ToLoadStorageService.Models;

namespace SitesGatherer.Sevices.SitesStorageService
{
    public interface ISitesStorage
    {
        public void StorePage(ParsedPage parsedPage, ToLoad toLoad);

        public bool LimitReached(string domain);

        public Dictionary<string, string> GetSitesData();
        public bool Contains(string domain, string[] pathParts);
        public void ResetIterator();
        public Page? GetNextPage();
    }
}