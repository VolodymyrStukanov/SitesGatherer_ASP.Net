namespace SitesGatherer.Sevices.Serialization.ModelsDTO
{
    public class ToLoadStorageDto
    {
        public IEnumerable<ToLoadDto> ToLoadDtos { get; init; } = [];
        public IEnumerable<string> IgnoredDomain { get; set; } = [];
    }
}