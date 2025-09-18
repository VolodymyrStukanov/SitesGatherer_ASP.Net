using SitesGatherer.Controllers.Setup.models;
using SitesGatherer.Sevices.Assets;

namespace SitesGatherer.Sevices.Settings
{
    public class SettingsService : ISettingsService
    {
        private List<string> startUrls = [];
        public List<string> StartUrls => this.startUrls;
        private int parentshipDepth = 0;
        public int ParentshipDepth => this.parentshipDepth;
        private List<string> prohibitedUrls = [];
        public List<string> ProhibitedUrls => this.prohibitedUrls;

        public void SetConfigs(ConfigModel configs)
        {
            this.startUrls = configs.StartUrls ?? [];
            this.prohibitedUrls = configs.ProhibitedUrls ?? [];
            this.parentshipDepth = configs.ParentshipDepth ?? 0;
        }

        public string? GetToLoadStorageJSON()
        {
            if (File.Exists($@"{Locations.ToLoadStroragePath}\{Locations.ToLoadFile}"))
                return File.ReadAllText($@"{Locations.ToLoadStroragePath}\{Locations.ToLoadFile}");
            return null;
        }

        public string? GetParsedStorageJSON()
        {
            if (File.Exists($@"{Locations.ProcessedPath}\{Locations.ProcessedFile}"))
                return File.ReadAllText($@"{Locations.ProcessedPath}\{Locations.ProcessedFile}");
            return null;
        }

        public string? GetSkippedStorageJSON()
        {
            if (File.Exists($@"{Locations.SkippedPath}\{Locations.SkippedFile}"))
                return File.ReadAllText($@"{Locations.SkippedPath}\{Locations.SkippedFile}");
            return null;
        }
    }
}