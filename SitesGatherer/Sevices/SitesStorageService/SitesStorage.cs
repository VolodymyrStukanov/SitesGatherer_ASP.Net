
using SitesGatherer.Sevices.HTMLParser.Models;
using SitesGatherer.Sevices.SitesStorageService.Models;
using SitesGatherer.Sevices.ToLoadStorageService.Models;

namespace SitesGatherer.Sevices.SitesStorageService
{
    public class SitesStorage : ISitesStorage
    {
        private int siteIterator = 0;
        private readonly List<Site> sites = [];
        private readonly Lock lockObject = new();

        public void StorePage(ParsedPage parsedPage, ToLoad toLoad)
        {
            lock (lockObject)
            {
                var site = this.sites.Find(x => x.Domain == toLoad.Domain);
                if (site == null)
                {
                    site = new Site(toLoad.Domain, toLoad.ParentDomain);
                    this.sites.Add(site);
                }
                site.AddPage(parsedPage, toLoad.PathParts, toLoad.ParentDomain);
            }
        }

        public bool LimitReached(string domain)
        {
            var site = this.sites.Find(x => x.Domain == domain);
            return site != null && site.LimitReached();
        }

        public bool Contains(string domain, string[] pathParts)
        {
            var site = this.sites.Find(x => x.Domain == domain);
            if (site == null) return false;
            return site.Contains(pathParts);
        }

        public Dictionary<string, string> GetSitesData() => this.sites.ToDictionary(x => x.Domain, x => x.GetUnitedData());

        public Page? GetNextPage()
        {
            lock (lockObject)
            {
                if (this.siteIterator < this.sites.Count)
                {
                    var page = this.sites[siteIterator].GetNextPage();
                    while (page == null && ++this.siteIterator < this.sites.Count)
                    {
                        page = this.sites[siteIterator].GetNextPage();                        
                    }
                    return page;
                }
                return null;
            }
        }

        public void ResetIterator()
        {
            lock (lockObject)
            {
                this.siteIterator = 0;
                foreach (var site in this.sites)
                {
                    site.ResetIterator();
                }
            }
        }
    }
}