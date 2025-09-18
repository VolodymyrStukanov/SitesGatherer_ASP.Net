
using SitesGatherer.Controllers.Setup.models;

namespace SitesGatherer.Sevices.Settings
{
    public interface ISettingsService
    {
        public List<string> ProhibitedUrls { get; }
        public int ParentshipDepth { get; }
        public List<string> StartUrls { get; }

        public void SetConfigs(ConfigModel configs);
        public string? GetToLoadStorageJSON();
        public string? GetParsedStorageJSON();
        public string? GetSkippedStorageJSON();
    }
}