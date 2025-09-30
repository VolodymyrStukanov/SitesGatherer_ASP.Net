using SitesGatherer.Sevices.Assets;
using SitesGatherer.Sevices.Serialization.Extensions;
using SitesGatherer.Sevices.SitesStorageService.Interfaces;
using SitesGatherer.Sevices.ToLoadStorageService;

namespace SitesGatherer.Sevices.DataStorageService
{
    public class DataSavier
    {
        private readonly ISitesStorage sitesStorage;
        private readonly IToLoadStorage toLoadStorage;

        private readonly Lock lockObject = new();
        public DataSavier(ISitesStorage sitesStorage, IToLoadStorage toLoadStorage)
        {
            this.sitesStorage = sitesStorage;
            this.toLoadStorage = toLoadStorage;
        }

        public void Save()
        {
            try
            {
                lock (lockObject)
                {
                    //зберігання інформації про те що вже обробили
                    Directory.CreateDirectory(Locations.ProcessedPath);
                    File.WriteAllText($@"{Locations.ProcessedPath}\{Locations.ProcessedFile}", this.sitesStorage.ToJson());
                    Directory.CreateDirectory(Locations.ToLoadStroragePath);
                    File.WriteAllText($@"{Locations.ToLoadStroragePath}\{Locations.ToLoadFile}", this.toLoadStorage.ToJson());

                    //зберігання контенту сайтів
                    var contentPath = $@"{Locations.ProcessedPath}\content";
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excetion while saving data. \nMessage: {ex.Message}");
            }
        }
    }
}