
namespace SitesGatherer.Sevices.HTMLParser.Models
{
    public class ParsedPage
    {
        public List<string> Links { get; }
        public List<string> Emails { get; }
        public List<string> PhoneNumbers { get; }
        public string Text { get; }
        public string Language { get; }
        
        public ParsedPage(List<string> links, List<string> emails, List<string> phoneNumbers, string text, string language)
        {
            this.Emails = emails;
            this.Language = language;
            this.Links = links;
            this.Text = text;
            this.PhoneNumbers = phoneNumbers;
        }

    }
}