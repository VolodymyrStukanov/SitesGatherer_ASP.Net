using SitesGatherer.Sevices.LeadsService.models;
using SitesGatherer.Sevices.Settings;
using SitesGatherer.Sevices.SitesStorageService.Interfaces;
using SitesGatherer.Sevices.SitesStorageService.Models;

namespace SitesGatherer.Sevices.LeadsService
{
    public class LeadsGenerator : ILeadsGenerator
    {
        private readonly ISitesStorage sitesStorage;
        private readonly ISettingsService settings;
        public LeadsGenerator(ISitesStorage sitesStorage, ISettingsService settings)
        {
            this.sitesStorage = sitesStorage;
            this.settings = settings;
        }

        private readonly short catalogContactsCountLimit = 6;
        public List<Lead> GetLeads()
        {
            var result = new List<Lead>();

            this.sitesStorage.ResetIterator();
            Page? page = this.sitesStorage.GetNextPage();
            while (page != null)
            {
                if (page.CanBeLead(catalogContactsCountLimit, settings.ProhibitedUrls))
                {
                    var newLead = new Lead(page.Payload!.PhoneNumbers, page.Payload.Emails, page.FullRoute!);
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
                page = this.sitesStorage.GetNextPage();
            }
            return result;
        }
    }
}