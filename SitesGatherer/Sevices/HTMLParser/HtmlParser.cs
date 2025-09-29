
using HtmlAgilityPack;
using Panlingo.LanguageIdentification.CLD2;
using AngleSharp;
using SitesGatherer.Sevices.HTMLParser.Models;
using System.Text.RegularExpressions;
using SitesGatherer.Extensions;

namespace SitesGatherer.Sevices.HTMLParser
{
    public class HtmlParser : IHtmlParser
    {
        private readonly List<string> startWithArray = ["380", "0", "38"];
        private readonly List<string> phonePatterns = new List<string>
        {
            @"(?<!\d)(?:(?:\+|00)?38\s*)?\d{2,5}(?:\s*\d{2,4}){1,5}(?!\d)"
        };

        private readonly List<string> emailPatterns = new List<string>
        {
            @"[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}",
            @"[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9\-]+\.[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}",
            @"[a-zA-Z0-9._%+\-]+\s?\[?at\]?\s?[a-zA-Z0-9.\-]+\s?\[?dot\]?\s?[a-zA-Z]{2,}",
            @"[a-zA-Z0-9._%+\-]+\s?\(?at\)?\s?[a-zA-Z0-9.\-]+\s?\(?dot\)?\s?[a-zA-Z]{2,}"
        };

        public async Task<ParsedPage> Parse(string html, string baseUrl)
        {
            string text = ExtractTextFromHtml(html);
            string normalized = NormalizeText(text);
            string lang = GetLanguageOfPageAsync(normalized);
            List<string> links = await GetLinks(html, baseUrl);
            // string testText = "38 (098) 5433354 asd asd  38 (0564) 001368. 38 (05632) 63493 asd sd (099)4818101 asd asd 0963803127 asd asd (067) 1201029" +
            // "asd asd (099) 26-22-122 asd asd (067) -577-16-09 asd asd 38(067)381-05-23. 38 ‎(098) 409 65 63, " +
            // "38 093 548-09-68 asd asd 38 (067) 313-77-17 asd asd 00 38 032 239 37 16.";
            // string testText = "semagro.info@ukr.net asd asd ad ad ad a beresuk_71@ukr.net,,, ,. agrourlex@i.ua. greenfuture1@ukr.net abracadabra, info@hope-center.biz,sidatimran75@gmail.com,fg.meyves@ukr.net, "
            // + "2016risbessarabii@ukr.net, info@ropa.in.ua text123 info@ukrsadvinprom.com. agro-gps-servis@ukr.net.  plastic.solutions.ua@gmail.com. ogneupor7@ukr.net. intm@intm.com.ua. text3123123 ivan.kostiuk@mega-elit.com.ua textTest321123 "
            // + " sale@liftec-group.com.ua          247-25-47@ukr.net           v.i.studia@gmail.com asdasdaasd pillars@i.ua 5381241@stud.kai.edu.ua";
            List<string> emails = ExtractMatches(text, emailPatterns);
            List<string> phoneNumbers = ExtractPhoneNumbers(text);
            return new ParsedPage(links, emails, phoneNumbers, text, lang);
        }

        private List<string> ExtractPhoneNumbers(string input)
        {
            string normalized = NormalizeForPhones(input);
            var segments = normalized.Split([" | "], StringSplitOptions.RemoveEmptyEntries);

            var numbers = segments.Select(segment => ExtractMatches(segment, phonePatterns))
            .SelectMany(x => x.Select(x => x))
            .Where(x =>
            {
                string phoneNumber = x.GetNumbers();
                if (phoneNumber.Length < 10) return false;
                if (!startWithArray.Any(start => phoneNumber.StartsWith(start))) return false;
                return true;
            }).ToList();

            var pattern = @"[^\p{N}]";
            return [.. numbers.Select(x => Regex.Replace(x, pattern, ""))];
        }

        public static string NormalizeForPhones(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            string normalized = input;

            // Normalize invisible / non-breaking spaces to normal spaces
            normalized = normalized.Replace("\u200E", " ")
                .Replace("\u200F", " ")
                .Replace("\u200B", " ")
                .Replace("\uFEFF", " ")
                .Replace("\u00A0", " ")
                .Replace("\u202F", " ");

            // Replace punctuation that usually separates sentences/numbers with a HARD separator " | "
            normalized = Regex.Replace(normalized, @"[.,;:!\r\n]+", " | ");

            // Replace characters used inside phone formatting with a space
            normalized = Regex.Replace(normalized, @"[-/()\[\]{}«»""'·—–]", " ");

            // Collapse whitespace to single spaces
            normalized = Regex.Replace(normalized, @"\s+", " ");

            // Normalize pipe spacing: " | "
            normalized = Regex.Replace(normalized, @"\s*\|\s*", " | ");

            return normalized.Trim();
        }

        private List<string> ExtractMatches(string input, List<string> patterns)
        {
            var results = new HashSet<string>();
            var rxOptions = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;
            foreach (var pattern in patterns)
            {
                var matches = Regex.Matches(input, pattern, rxOptions);
                foreach (Match match in matches)
                {
                    string value = match.Value.Trim();
                    if (!string.IsNullOrWhiteSpace(value))
                        results.Add(value);
                }
            }
            return results.ToList();
        }

        private async Task<List<string>> GetLinks(string html, string baseUrl)
        {
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(req => req.Content(html));

            var attributesWithLinks = document.QuerySelectorAll("a");
            var links = new List<string>();
            foreach (var element in attributesWithLinks)
            {
                var href = element.GetAttribute("href");
                if (string.IsNullOrEmpty(href))
                    continue;

                if (Uri.TryCreate(href, UriKind.Absolute, out var uri))
                {
                    if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                        links.Add(uri.ToString());
                }
                else if (href != "/" && Uri.TryCreate(href.StartsWith('/') ? $"{baseUrl}{href}" : $"{baseUrl}/{href}", UriKind.Absolute, out uri))
                {
                    links.Add(uri.ToString());
                }
            }
            return links;
        }

        private string ExtractTextFromHtml(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            
            //removing all scripts and styles
            var garbage = doc.DocumentNode.SelectNodes("//script|//style");
            if (garbage != null)
            {
                foreach (var node in garbage)
                    node.Remove();
            }

            // Replace all HTML tags with their text content followed by a space
            HtmlNodeCollection? allElements = doc.DocumentNode.SelectNodes("//*");
            if (allElements != null)
            {
                // Process from innermost to outermost (reverse order)
                for (int i = allElements.Count - 1; i >= 0; i--)
                {
                    var element = allElements[i];
                    
                    if (element.Name == "br")
                    {
                        // Replace <br> with space
                        var spaceNode = doc.CreateTextNode(" ");
                        element.ParentNode.ReplaceChild(spaceNode, element);
                    }
                    else if (element.HasChildNodes)
                    {
                        // Add space after element's content
                        var textContent = element.InnerText;
                        if (!string.IsNullOrWhiteSpace(textContent))
                        {
                            var newTextNode = doc.CreateTextNode(textContent + " ");
                            element.ParentNode.ReplaceChild(newTextNode, element);
                        }
                        else
                        {
                            element.Remove();
                        }
                    }
                }
            }
            
            var text = Regex.Replace(doc.DocumentNode.InnerText, @"\s+", " ");
            return text;
        }

        public static string NormalizeText(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Replace invisible and non-breaking spaces with normal space
            string result = input
                .Replace("\u200E", " ") // Left-to-right mark
                .Replace("\u200F", " ") // Right-to-left mark
                .Replace("\u200B", " ") // Zero-width space
                .Replace("\uFEFF", " ") // Zero-width no-break space
                .Replace("\u00A0", " ") // Non-breaking space
                .Replace("\u202F", " "); // Narrow non-breaking space

            // Replace separators with spaces
            result = Regex.Replace(result, @"[-()./]", " ");

            // Collapse multiple spaces into a single space
            result = Regex.Replace(result, @"\s+", " ");

            // Trim spaces at the ends
            return result.Trim();
        }

        private string GetLanguageOfPageAsync(string text)
        {
            var detector = new CLD2Detector();
            var predictions = detector.PredictLanguage(text).OrderByDescending(x => x.Proportion);
            return predictions.First().Language;
        }
        
    }
}