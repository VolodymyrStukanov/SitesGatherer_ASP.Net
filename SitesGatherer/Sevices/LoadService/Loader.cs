using System.Text;

namespace SitesGatherer.Sevices.LoadService
{
    public class Loader : ILoader
    {
        private readonly HttpClient client;

        public Loader(HttpClient client)
        {
            this.client = client;
        }

        public async Task<string> LoadPage(string url)
        {
            try
            {
                using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                var contentType = response.Content.Headers.ContentType?.MediaType?.ToLower() ?? "";
                if (!IsHtmlContent(contentType))
                {
                    Console.WriteLine($"Skipping non-HTML content: {contentType} for URL: {url}");
                    return string.Empty;
                }

                var bytes = await response.Content.ReadAsByteArrayAsync();
                // Try to detect encoding, fallback to UTF-8
                var charset = response.Content.Headers.ContentType?.CharSet;
                Encoding encoding;
                try
                {
                    encoding = !string.IsNullOrWhiteSpace(charset)
                        ? Encoding.GetEncoding(charset.Trim('"'))  // remove possible quotes
                        : Encoding.UTF8;
                }
                catch
                {
                    encoding = Encoding.UTF8; // fallback if invalid
                }

                string html = encoding.GetString(bytes);
                return html;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excetion while downdloading a page. \nMessage: {ex.Message}\nUrl: {url}");
                return String.Empty;
            }
        }
        
        private bool IsHtmlContent(string contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType))
                return true; // Assume HTML if no content type specified
            
            contentType = contentType.ToLower();
            
            // Allow only HTML and text-based content
            return contentType.Contains("text/html") ||
                contentType.Contains("application/xhtml") ||
                contentType.Contains("text/plain") ||
                contentType.Contains("text/xml") ||
                contentType.Contains("application/xml");
        }
    }
}