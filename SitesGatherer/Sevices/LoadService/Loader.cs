
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

                // string html = await client.GetStringAsync(url);
                return html;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excetion while downdloading a page. \nMessage: {ex.Message}\nUrl: {url}");
                return String.Empty;
            }
        }
    }
}