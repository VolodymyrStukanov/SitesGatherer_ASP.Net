
namespace SitesGatherer.Sevices.SitesStorageService.Models
{
    public class Page
    {
        private int childrenInteration = 0;
        private PageContent? content;
        public PageContent? Content => this.content;
        private readonly List<Page> childPages = new();
        private readonly string route;
        public string Route => this.route;
        private readonly string? fullRoute;
        public string? FullRoute => this.fullRoute;

        public Page(string route, string content, List<string>? phoneNumbers, List<string>? emails)
        {
            this.route = route;
            this.content = new PageContent(content, phoneNumbers, emails);
        }

        public Page(string route)
        {
            this.route = route;
        }

        public Page(Page original, string? parentRoute = null)
        {
            this.childrenInteration = 0;
            this.content = original.content == null ? null : new PageContent(original.content);
            this.route = original.route;
            if (original.fullRoute != null)
                this.fullRoute = original.fullRoute.StartsWith('?') ? $"{parentRoute}{original.fullRoute}" : $"{parentRoute}/{original.fullRoute}";
            else
                this.fullRoute = this.route.StartsWith('?') ? $"{parentRoute}{this.route}" : $"{parentRoute}/{this.route}";
        }

        public int GetChildPagesCount()
        {
            return this.childPages.Count;
        }

        public void AddChildPage(Page page)
        {
            this.childPages.Add(page);
        }

        public Page? GetChildByRoute(string route)
        {
            return this.childPages.Find(x => x.route == route);
        }

        public bool FindChild(string[] pathParts)
        {
            if (pathParts.Length == 0) return true;
            var child = this.childPages.Find(x => x.route == pathParts[0]);
            if (child == null) return false;
            return child.FindChild(pathParts[1..]);
        }

        public string GetContentWithChildren()
        {
            var childrenContent = String.Join('\n', this.childPages.Select(x =>
            {
                x.GetContentWithChildren();
                return true;
            }));
            return $"{this.content?.Content}\n{childrenContent}";
        }

        public void SetContent(string content, List<string>? emails, List<string>? phoneNumbers)
        {
            this.content = new PageContent(content, phoneNumbers, emails);
        }

        public Page GetNextPage()
        {
            var page = childrenInteration >= this.childPages.Count ? null : this.childPages[childrenInteration];
            if (page != null)
            {
                var nextChildren = page.GetNextPage();
                if (nextChildren == page)
                    childrenInteration++;
                var copy = new Page(nextChildren, this.route);
                return copy;
            }
            childrenInteration = 0;
            return this;
        }

        public void ResetIterator()
        {
            this.childrenInteration = 0;
            foreach (var page in this.childPages)
            {
                page.ResetIterator();
            }
        }

        public bool CanBeLead(short catalogContactsCountLimit, List<string> prohibitedUrls)
        {
            var contentNotNull = this.Content != null
                && this.Content.Emails.Count + this.Content.PhoneNumbers.Count > 0;
            var meetLimitation = contentNotNull && fullRoute != null;
            meetLimitation = meetLimitation && !prohibitedUrls.Any(fullRoute!.Contains);
            meetLimitation = meetLimitation && (this.Content!.Emails.Count + this.Content.PhoneNumbers.Count <= catalogContactsCountLimit);
            
            return contentNotNull && meetLimitation;
        }
    }
}