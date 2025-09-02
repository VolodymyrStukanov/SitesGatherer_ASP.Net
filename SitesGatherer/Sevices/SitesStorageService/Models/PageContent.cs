
namespace SitesGatherer.Sevices.SitesStorageService.Models
{
    public class PageContent
    {
        public HashSet<string> PhoneNumbers { get; }
        public HashSet<string> Emails { get; }
        public string Content { get; }

        public PageContent(string content, List<string>? phoneNumbers, List<string>? emails)
        {
            this.Content = content;
            this.Emails = emails != null ? emails.ToHashSet() : [];
            this.PhoneNumbers = phoneNumbers != null ? phoneNumbers.ToHashSet() : [];
        }

        public PageContent(PageContent original )
        {
            PhoneNumbers = new HashSet<string>(original.PhoneNumbers);
            Emails = new HashSet<string>(original.Emails);
            Content = original.Content;
        }
    }
}