using SitesGatherer.Sevices.Serialization.Interfaces;
using SitesGatherer.Sevices.Serialization.ModelsDTO;
using SitesGatherer.Sevices.SitesStorageService.Models;

namespace SitesGatherer.Sevices.SitesStorageService.Factories
{
    public class PageFactory : IDeserializingFactory<Page, PageDto>
    {
        private readonly PayloadFactory payloadFactory = new();

        public static Page Default(string route, string content, List<string>? phoneNumbers, List<string>? emails)
        {
            return new Page(route, new Payload(content, phoneNumbers, emails));
        }

        public static Page AsChild(Page original, string? parentRoute = null)
        {
            var payload = original.Payload == null ? null : new Payload(original.Payload);
            var route = original.route;
            string? fullRoute;
            if (original.FullRoute != null)
                fullRoute = original.FullRoute.StartsWith('?') ? $"{parentRoute}{original.FullRoute}" : $"{parentRoute}/{original.FullRoute}";
            else
                fullRoute = route.StartsWith('?') ? $"{parentRoute}{route}" : $"{parentRoute}/{route}";

            return new Page(route, fullRoute, payload);
        }

        public Page FromDto(PageDto dto)
        {   
            var page = new Page(dto.Route);
            
            if (dto.Payload != null)
            {
                var payload = payloadFactory.FromDto(dto.Payload);
                page.SetPayload(payload);
            }
            
            foreach (var childDto in dto.ChildPages)
            {
                var childPage = FromDto(childDto);
                page.AddChildPage(childPage);
            }
            
            return page;
        }
    }
}