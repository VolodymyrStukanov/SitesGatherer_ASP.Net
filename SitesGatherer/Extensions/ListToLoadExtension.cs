using SitesGatherer.Sevices.ToLoadStorageService.Models;

namespace SitesGatherer.Extensions
{
    public static class ListToLoadExtension
    {
        public static IEnumerable<ToLoad> GetToLoads(this List<string> urls, string? domain = null, int? parentshipDepth = null)
        {
            return urls.Select(url => new ToLoad(url, domain, parentshipDepth));
        }
    }
}