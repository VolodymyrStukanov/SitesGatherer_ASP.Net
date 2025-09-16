using SitesGatherer.Sevices.Serialization.Interfaces;
using SitesGatherer.Sevices.Serialization.ModelsDTO;
using SitesGatherer.Sevices.SitesStorageService.Models;

namespace SitesGatherer.Sevices.SitesStorageService.Factories
{
    public class PayloadFactory : IDeserializingFactory<Payload, PayloadDto>
    {
        public Payload FromDto(PayloadDto dto)
        {
            return new Payload(dto.PhoneNumbers, dto.Emails);
        }
    }
}