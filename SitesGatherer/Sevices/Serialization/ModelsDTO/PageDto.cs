namespace SitesGatherer.Sevices.Serialization.ModelsDTO
{
    public record PageDto
    {
        public PayloadDto? Payload { get; init; }
        public List<PageDto> ChildPages { get; init; } = new();
        public string Route { get; init; } = "";
    }
}