namespace SitesGatherer.Sevices.Serialization.ModelsDTO
{
    public record ToLoadDto
    {
        public string Link { get; set; } = "";
        public string? ParentDomain { get; set; }
        public int? ParentshipDepth { get; set; }
    }
}