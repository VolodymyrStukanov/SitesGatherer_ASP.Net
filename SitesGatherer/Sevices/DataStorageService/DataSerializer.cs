using Newtonsoft.Json;
using SitesGatherer.Sevices.Serialization.Extensions;
using SitesGatherer.Sevices.SitesStorageService;
using SitesGatherer.Sevices.ToLoadStorageService;

namespace SitesGatherer.Sevices.DataStorageService
{
    public class DataSavier
    {
        string baseDirectory = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;

        private readonly ISitesStorage sitesStorage;
        private readonly IToLoadStorage toLoadStorage;
        public DataSavier(ISitesStorage sitesStorage, IToLoadStorage toLoadStorage)
        {
            this.sitesStorage = sitesStorage;
            this.toLoadStorage = toLoadStorage;
        }

        public void SaveProcessed()
        {
            string path = $@"{baseDirectory}\data\processed";

            //зберігання інформації про те що вже обробили
            Directory.CreateDirectory(path);
            File.WriteAllText($@"{path}\processed_storage.txt", this.sitesStorage.ToJson());

            //зберігання контенту сайтів
            var contentPath = $@"{path}\content";
            Directory.CreateDirectory(contentPath);
            var sites = this.sitesStorage.GetSitesData();

            foreach (var site in sites)
            {
                if (File.Exists($@"{contentPath}\{site.Key}"))
                    File.AppendAllText($@"{contentPath}\{site.Key}.json", site.Value);
                else
                    File.WriteAllText($@"{contentPath}\{site.Key}.json", site.Value);
            }

        }

        public void Save()
        {
            string processedSitesFolder = $@"{baseDirectory}\data\processed";
            string skippedSitesFolder = $@"{baseDirectory}\data\skipped";
            string toLoadPagesFolder = $@"{baseDirectory}\data\toLoad";
            Directory.CreateDirectory(processedSitesFolder);
            Directory.CreateDirectory(skippedSitesFolder);
            Directory.CreateDirectory(toLoadPagesFolder);

            File.WriteAllText($@"{processedSitesFolder}\testFile1.txt", "Test text");

            string jsonString = JsonConvert.SerializeObject(this.sitesStorage, Formatting.Indented);
            var a = 10;
        }
    }
}