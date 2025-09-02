using SitesGatherer.Sevices.LeadsService.models;

namespace SitesGatherer.Sevices.LeadsService
{
    public interface ILeadsGenerator
    {
        public List<Lead> GetLeads();
    }
}