
namespace SitesGatherer.Sevices.ToLoadStorageService.Models
{
    public class ToLoad
    {
        public string Link { get; }
        public string[] PathParts { get; }
        public string Domain { get; }
        public string BaseUrl { get; }
        public string? ParentDomain { get; }
        public int LinkHash { get; }
        public int? ParentshipDepth { get; }

        public ToLoad(){}
        public ToLoad(string link, string? parentDomain = null, int? parentshipDepth = null)
        {
            if (!Uri.TryCreate(link, UriKind.Absolute, out var uri))
                throw new ArgumentException("Invalid absolute URL", nameof(link));
        
            string scheme = uri.Scheme;
            string host = uri.Host;
            int port = uri.IsDefaultPort ? -1 : uri.Port;
            string path = uri.AbsolutePath.TrimEnd('/');
            string query = uri.Query;
            string[] pathParts = uri.PathAndQuery
                .Trim('/')
                .Split(new char[] { '/', '?' }, StringSplitOptions.RemoveEmptyEntries);

            var builder = new UriBuilder
            {
                Scheme = scheme,
                Host = host,
                Port = port,
                Path = path,
                Query = query,
                Fragment = ""   // remove #fragment
            };

            this.PathParts = pathParts;
            this.Link = builder.Uri.ToString();
            this.LinkHash = this.Link.GetHashCode();
            this.ParentDomain = parentDomain;
            this.Domain = host;
            this.BaseUrl = $"{scheme}://{host}";
            if (parentDomain == host)
            {
                this.ParentshipDepth = parentshipDepth;
            }
            else
            {
                this.ParentshipDepth = parentshipDepth - 1 < 1 ? 0 : parentshipDepth - 1;
            }
        }
    }
}