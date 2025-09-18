using SitesGatherer.Sevices.Assets;
using SitesGatherer.Sevices.Serialization.Extensions;
using SitesGatherer.Sevices.SitesStorageService.Interfaces;
using SitesGatherer.Sevices.ToLoadStorageService;

namespace SitesGatherer.Sevices.DataStorageService
{
    public class DataSavier
    {
        private readonly IParsedStorage parsedStorage;
        private readonly ISkippedStorage skippedStorage;
        private readonly IToLoadStorage toLoadStorage;
        public DataSavier(IParsedStorage parsedStorage, ISkippedStorage skippedStorage, IToLoadStorage toLoadStorage)
        {
            this.parsedStorage = parsedStorage;
            this.skippedStorage = skippedStorage;
            this.toLoadStorage = toLoadStorage;
        }

        public void Save()
        {

            //зберігання інформації про те що вже обробили
            Directory.CreateDirectory(Locations.ProcessedPath);
            File.WriteAllText($@"{Locations.ProcessedPath}\{Locations.ProcessedFile}", this.parsedStorage.ToJson());
            Directory.CreateDirectory(Locations.SkippedPath);
            File.WriteAllText($@"{Locations.SkippedPath}\{Locations.SkippedFile}", this.skippedStorage.ToJson());
            Directory.CreateDirectory(Locations.ToLoadStroragePath);
            File.WriteAllText($@"{Locations.ToLoadStroragePath}\{Locations.ToLoadFile}", this.toLoadStorage.ToJson());

            //зберігання контенту сайтів
            var contentPath = $@"{Locations.ProcessedPath}\content";
            Directory.CreateDirectory(contentPath);
            var sites = this.parsedStorage.GetSitesData();

            foreach (var site in sites)
            {
                if (File.Exists($@"{contentPath}\{site.Key}"))
                    File.AppendAllText($@"{contentPath}\{site.Key}.json", site.Value);
                else
                    File.WriteAllText($@"{contentPath}\{site.Key}.json", site.Value);
            }

        }
    }
}