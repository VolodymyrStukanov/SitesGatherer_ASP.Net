using SitesGatherer.Sevices.Serialization.Interfaces;
using SitesGatherer.Sevices.Serialization.ModelsDTO;
using SitesGatherer.Sevices.SitesStorageService.Models;

namespace SitesGatherer.Sevices.SitesStorageService.Factories
{
    public class SiteFactory : IDeserializingFactory<Site, SiteDto>
    {
        private readonly PageFactory pageFactory = new();

        public Site FromDto(SiteDto dto)
        {

            //ТРЕБА ВИГАДАТИ НАЩО МЕНІ КАЛЬКУЛЯЦІЯ КІЛЬКОСТІ СТОРІНОК
            //ЯКЩО ВОНА ПОТРІБНА, ТРЕБА ВИГАДАТИ ЯК РЕСТОРИТИ ЇЇ З ЗБЕРЕЖЕНОГО JSON ФАЙЛУ
            //site.SetPagesCount(dto.PagesCount);

            Site site;
            if (dto.HomePage != null) site = new Site(dto.Domain, pageFactory.FromDto(dto.HomePage));
            else site = new Site(dto.Domain);
            
            return site;
        }
    }
}