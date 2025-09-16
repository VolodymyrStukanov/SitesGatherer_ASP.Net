using SitesGatherer.Sevices.Serialization.Interfaces;
using SitesGatherer.Sevices.Serialization.ModelsDTO;

namespace SitesGatherer.Sevices.SitesStorageService.Models
{
    public class Payload : ISerializableData<PayloadDto>
    {
        public HashSet<string> PhoneNumbers { get; }
        public HashSet<string> Emails { get; }
        public string? Content { get; }

        public Payload(List<string>? phoneNumbers, List<string>? emails)
        {
            this.Emails = emails != null ? emails.ToHashSet() : [];
            this.PhoneNumbers = phoneNumbers != null ? phoneNumbers.ToHashSet() : [];
        }

        public Payload(string content, List<string>? phoneNumbers, List<string>? emails)
        {
            this.Content = content;
            this.Emails = emails != null ? emails.ToHashSet() : [];
            this.PhoneNumbers = phoneNumbers != null ? phoneNumbers.ToHashSet() : [];
        }

        public Payload(Payload original)
        {
            PhoneNumbers = new HashSet<string>(original.PhoneNumbers);
            Emails = new HashSet<string>(original.Emails);
            Content = original.Content;
        }

        public PayloadDto ToDto()
        {
            return new PayloadDto
            {
                PhoneNumbers = this.PhoneNumbers.ToList(),
                Emails = this.Emails.ToList()
            };
        }
    }
}