

using SitesGatherer.Sevices.HTMLParser.Models;

namespace SitesGatherer.Sevices.SitesStorageService.Models
{
    public class Site
    {
        public string Domain { get; }
        private readonly Page homePage;
        private readonly int pageLimit;
        private readonly int pagesCount = 0;
        private readonly List<string> parentDomains;
        private bool allPagesIterated = false;

        private readonly Lock lockObject = new();
        public Site(string domain, string? parent = null, int pageLimit = 100)
        {
            this.Domain = domain;
            this.homePage = new Page(domain);
            this.pageLimit = pageLimit;
            this.parentDomains = [domain];
        }

        public void AddPage(ParsedPage parsedPage, string[] pathParts, string? parentDomain = null)
        {
            lock (lockObject)
            {
                if (parentDomain != null && !this.parentDomains.Contains(parentDomain)) this.parentDomains.Add(parentDomain);
                var currentPage = this.homePage;
                if (currentPage.GetChildPagesCount() == 0)
                {
                    foreach (var part in pathParts)
                    {
                        var newPage = new Page(part);
                        currentPage.AddChildPage(newPage);
                        currentPage = newPage;
                    }
                    currentPage.SetContent(parsedPage.Text, parsedPage.Emails, parsedPage.PhoneNumbers);
                }
                else
                {
                    int i = 0;
                    foreach (var x in pathParts)
                    {
                        var nextPage = currentPage.GetChildByRoute(x);
                        if (nextPage == null) break;
                        currentPage = nextPage;
                        i++;
                    }
                    if (i != pathParts.Length)
                    {
                        var newPage = new Page(pathParts[i], parsedPage.Text, parsedPage.PhoneNumbers, parsedPage.Emails);
                        currentPage.AddChildPage(newPage);
                    }
                    else
                    {
                        currentPage.SetContent(parsedPage.Text, parsedPage.Emails, parsedPage.PhoneNumbers);
                    }
                }
            }
        }

        public bool LimitReached()
        {
            lock (lockObject)
            {
                return this.pagesCount >= this.pageLimit;
            }
        }

        public bool Contains(string[] pathParts) => this.homePage.FindChild(pathParts);

        public string GetUnitedData() => this.homePage.GetContentWithChildren();

        public Page? GetNextPage()
        {
            if (allPagesIterated) return null;

            var page = this.homePage.GetNextPage();
            if (page == this.homePage)
                this.allPagesIterated = true;
            return page;
        }
        public void ResetIterator()
        {
            this.allPagesIterated = false;
            this.homePage.ResetIterator();
        }
    }
}