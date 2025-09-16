using SitesGatherer.Sevices.Serialization.Interfaces;
using SitesGatherer.Sevices.Serialization.ModelsDTO;

namespace SitesGatherer.Sevices.SitesStorageService.Factories
{
    public class SiteStorageFactory : IDeserializingFactory<SitesStorage, SitesStorageDto>
    {
        private readonly SiteFactory siteFactory = new();
        public SitesStorage FromDto(SitesStorageDto dto)
        {
            var storage = new SitesStorage();
            storage.Restore(dto.Sites.Select(x => siteFactory.FromDto(x)));
            return storage;
        }
    }
}