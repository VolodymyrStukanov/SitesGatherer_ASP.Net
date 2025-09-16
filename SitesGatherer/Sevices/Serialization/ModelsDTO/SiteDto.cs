
namespace SitesGatherer.Sevices.Serialization.ModelsDTO
{
    public record SiteDto
    {
        public string Domain { get; init; } = "";
        public PageDto? HomePage { get; init; }
        public int PagesCount { get; init; }
        public List<string> ParentDomains { get; init; } = [];
        public bool AllPagesIterated { get; init; }
        
    }
}