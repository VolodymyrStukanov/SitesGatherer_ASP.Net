
namespace SitesGatherer.Controllers.Setup.models
{
    public class ConfigModel
    {        
        public List<string>? StartUrls { get; set; }
        public List<string>? ProhibitedUrls { get; set; }
        public int? ParentshipDepth { get; set; }
    }
}