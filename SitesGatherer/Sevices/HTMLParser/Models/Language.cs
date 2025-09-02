
namespace SitesGatherer.Sevices.HTMLParser.Models
{
    public class Language
    {
        private readonly string lang;

        public Language(string lang)
        {
            this.lang = lang;
        }

        public static bool operator ==(Language langObj, string lang) {
            return langObj.lang == lang;
        }

        public static bool operator ==(string lang, Language langObj) {
            return langObj.lang == lang;
        }

        public static bool operator !=(Language langObj, string lang) {
            return langObj.lang != lang;
        }

        public static bool operator !=(string lang, Language langObj) {
            return langObj.lang != lang;
        }
    }
}