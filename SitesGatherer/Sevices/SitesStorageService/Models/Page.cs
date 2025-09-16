using SitesGatherer.Sevices.Serialization.Interfaces;
using SitesGatherer.Sevices.Serialization.ModelsDTO;
using SitesGatherer.Sevices.SitesStorageService.Factories;

namespace SitesGatherer.Sevices.SitesStorageService.Models
{
    public class Page : ISerializableData<PageDto>
    {
        private int childrenInteration = 0;
        private Payload? payload;
        public Payload? Payload => this.payload;
        private readonly List<Page> childPages = new();
        public readonly string route;
        public string Route => this.route;
        private readonly string? fullRoute;
        public string? FullRoute => this.fullRoute;
        
        private readonly Lock lockObject = new();

        public Page(){}

        public Page(string route)
        {
            this.route = route;
        }
        
        public Page(string route, Payload payload)
        {
            this.payload = payload;
            this.route = route;
        }

        public Page(string route, string fullRoute, Payload? payload)
        {
            this.payload = payload;
            this.route = route;
            this.fullRoute = fullRoute;
        }

        public int GetChildPagesCount()
        {
            return this.childPages.Count;
        }

        public void AddChildPage(Page page)
        {
            lock (lockObject)
            {
                this.childPages.Add(page);
            }
        }

        public Page? GetChildByRoute(string route)
        {
            lock (lockObject)
            {
                return this.childPages.Find(x => x.route == route);
            }
        }

        public bool FindChild(string[] pathParts)
        {
            lock (lockObject)
            {
                if (pathParts.Length == 0) return true;
                var child = this.childPages.Find(x => x.route == pathParts[0]);
                if (child == null) return false;
                return child.FindChild(pathParts[1..]);
            }
        }

        public string GetContentWithChildren()
        {
            lock (lockObject)
            {
                var childrenContent = String.Join('\n', this.childPages.Select(x =>
                {
                    return x.GetContentWithChildren();
                }));
                return $"{this.payload?.Content}\n{childrenContent}";
            }
        }

        public void SetPayload(string content, List<string>? emails, List<string>? phoneNumbers)
        {
            this.payload = new Payload(content, phoneNumbers, emails);
        }

        public void SetPayload(Payload payload)
        {
            this.payload = payload;
        }

        public Page GetNextChildPage()
        {
            lock (lockObject)
            {
                var page = childrenInteration >= this.childPages.Count ? null : this.childPages[childrenInteration];
                if (page != null)
                {
                    var nextChildren = page.GetNextChildPage();
                    if (nextChildren == page)
                        childrenInteration++;
                    var copy = PageFactory.AsChild(nextChildren, this.route);
                    return copy;
                }
                childrenInteration = 0;
                return this;
            }
        }

        public void ResetIterator()
        {
            lock (lockObject)
            {
                this.childrenInteration = 0;
                foreach (var page in this.childPages)
                {
                    page.ResetIterator();
                }
            }
        }

        public bool CanBeLead(short catalogContactsCountLimit, List<string> prohibitedUrls)
        {
            var contentNotNull = this.Payload != null
                && this.Payload.Emails.Count + this.Payload.PhoneNumbers.Count > 0;
            var meetLimitation = contentNotNull && fullRoute != null;
            meetLimitation = meetLimitation && !prohibitedUrls.Any(fullRoute!.Contains);
            meetLimitation = meetLimitation && (this.Payload!.Emails.Count + this.Payload.PhoneNumbers.Count <= catalogContactsCountLimit);
            
            return contentNotNull && meetLimitation;
        }

        public PageDto ToDto()
        {
            return new PageDto
            {
                Payload = payload?.ToDto(),
                ChildPages = childPages.Select(p => p.ToDto()).ToList(),
                Route = route
            };
        }
    }
}