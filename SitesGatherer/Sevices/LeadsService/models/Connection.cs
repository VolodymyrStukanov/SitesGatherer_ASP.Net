
namespace SitesGatherer.Sevices.LeadsService.models
{
    public class Connection
    {
        public string Contact { get; }
        public HashSet<string> Sources { get; }
        public Connection(string contact, HashSet<string> sources)
        {
            this.Contact = contact;
            this.Sources = new HashSet<string>(sources);
        }
    }
}