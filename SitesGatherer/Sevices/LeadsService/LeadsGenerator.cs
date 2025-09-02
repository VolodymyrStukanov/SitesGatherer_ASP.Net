using SitesGatherer.Sevices.LeadsService.models;
using SitesGatherer.Sevices.Settings;
using SitesGatherer.Sevices.SitesStorageService;
using SitesGatherer.Sevices.SitesStorageService.Models;

namespace SitesGatherer.Sevices.LeadsService
{
    public class LeadsGenerator : ILeadsGenerator
    {
        private readonly ISitesStorage parsedStorage;
        private readonly ISettingsService settings;
        public LeadsGenerator(ISitesStorage parsedStorage, ISettingsService settings)
        {
            this.parsedStorage = parsedStorage;
            this.settings = settings;
        }

        private readonly short catalogContactsCountLimit = 6;
        public List<Lead> GetLeads()
        {
            var result = new List<Lead>();

            this.parsedStorage.ResetIterator();
            Page? page = this.parsedStorage.GetNextPage();
            while (page != null)
            {
                if (page.CanBeLead(catalogContactsCountLimit, settings.ProhibitedUrls))
                {
                    var newLead = new Lead(page.Content!.PhoneNumbers, page.Content.Emails, page.FullRoute!);
                    if (result.Count == 0)
                    {
                        result.Add(newLead);
                    }
                    else
                    {
                        var added = false;
                        foreach (var lead in result)
                        {
                            if (!newLead.Coincident(lead)) continue;

                            result.Remove(lead);

                            var unitedLead = new Lead([lead, newLead]);
                            result.Add(unitedLead);
                            added = true;
                            break;
                        }
                        if (!added) result.Add(newLead);
                    }
                }
                page = this.parsedStorage.GetNextPage();
            }
            return result;
        }
    }
}