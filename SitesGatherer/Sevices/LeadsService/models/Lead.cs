using SitesGatherer.Extensions;

namespace SitesGatherer.Sevices.LeadsService.models
{
    public class Lead
    {
        private readonly List<Connection> phoneNumbers = [];
        public List<Connection> PhoneNumbers => this.phoneNumbers;
        private readonly List<Connection> emails = [];
        public List<Connection> Emails => this.emails;

        public Lead(HashSet<string>? phoneNumbers, HashSet<string>? emails, string sourcesUrl)
        {
            if (sourcesUrl == null) throw new Exception("sourcesUrl must be not null");
            
            this.phoneNumbers = phoneNumbers != null ? [.. phoneNumbers.Select(x => new Connection(x, [sourcesUrl]))] : [];
            this.emails = emails != null ? [.. emails.Select(x => new Connection(x, [sourcesUrl]))] : [];
        }

        private readonly short sourceLimit = 3;
        public Lead(List<Lead> leads)
        {
            leads.ForEach(x =>
            {
                if (this.emails.Any())
                {
                    foreach (var email in x.emails)
                    {
                        var res = this.emails.Find(x => x.Contact == email.Contact);
                        if (res != null && res.Sources.Count < sourceLimit)
                            res.Sources.AddRange(email.Sources);
                    }
                }
                else this.emails.AddRange(x.emails);

                if (this.phoneNumbers.Any())
                {
                    foreach (var number in x.phoneNumbers)
                    {
                        var res = this.phoneNumbers.Find(x => x.Contact == number.Contact);
                        if (res != null && res.Sources.Count < sourceLimit)
                            res.Sources.AddRange(number.Sources);
                    }
                }
                else this.phoneNumbers.AddRange(x.phoneNumbers);
            });
        }

        public bool Coincident(Lead toCompare)
        {
            foreach (var email in toCompare.emails)
            {
                var res = this.emails.Where(x => x.Contact == email.Contact);
                if (res.Any()) return true;
            }
            foreach (var number in toCompare.phoneNumbers)
            {
                var res = this.phoneNumbers.Where(x => x.Contact == number.Contact);
                if (res.Any()) return true;
            }
            return false;
        }
    }
}