
namespace SitesGatherer.Sevices.Serialization.ModelsDTO
{
    public record PayloadDto
    {
        public List<string>? PhoneNumbers { get; init; }
        public List<string>? Emails { get; init; }
        
    }
}