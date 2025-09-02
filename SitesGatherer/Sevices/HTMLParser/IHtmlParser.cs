using SitesGatherer.Sevices.HTMLParser.Models;

namespace SitesGatherer.Sevices.HTMLParser
{
    public interface IHtmlParser
    {
        public Task<ParsedPage> Parse(string html, string baseUrl);
    }
}