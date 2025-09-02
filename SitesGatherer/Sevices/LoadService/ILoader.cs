namespace SitesGatherer.Sevices.LoadService
{
    public interface ILoader
    {
        public Task<string> LoadPage(string url);
    }
}