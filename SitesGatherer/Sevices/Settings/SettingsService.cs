
using SitesGatherer.Controllers.Setup.models;

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
    }
}