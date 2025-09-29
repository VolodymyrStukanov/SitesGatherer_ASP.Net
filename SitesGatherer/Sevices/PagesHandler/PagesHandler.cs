using SitesGatherer.Sevices.HTMLParser;
using SitesGatherer.Sevices.LoadService;
using SitesGatherer.Sevices.ToLoadStorageService.Models;
using SitesGatherer.Sevices.ToLoadStorageService;
using SitesGatherer.Sevices.HTMLParser.Models;
using Microsoft.Extensions.Options;
using SitesGatherer.Sevices.Settings;
using SitesGatherer.Sevices.SitesStorageService.Interfaces;
using SitesGatherer.Sevices.DataStorageService;
using SitesGatherer.Sevices.WorkerService;
using SitesGatherer.Sevices.WorkerService.Models;
using SitesGatherer.Sevices.PagesHandler.models;

namespace SitesGatherer.Sevices.PagesHandler
{
    public class PagesHandler : IPagesHandler
    {
        private readonly Lock lockObject = new();
        private readonly List<string> domainsTakenByWorkers = [];
        private readonly WorkersManager workersManager;
        
        private readonly ILoader loader;
        private readonly ISitesStorage sitesStorage;
        private readonly List<string> allowedLanguages;
        private readonly IToLoadStorage toLoadStorage;
        private readonly IHtmlParser parser;
        private readonly ISettingsService settings;
        private readonly DataSavier dataSavier;
        private int pageCounter = 0;

        private readonly int delay = 5000;

        public PagesHandler(
            ILoader loader,
            ISitesStorage sitesStorage,
            IOptions<WorkerSettings> workerSettings,
            IOptions<LanguagesSettings> languagesSettings,
            ISettingsService settings,
            IToLoadStorage toLoadStorage,
            DataSavier dataSavier)
        {
            this.loader = loader;
            this.sitesStorage = sitesStorage;
            this.parser = new HtmlParser();
            this.toLoadStorage = toLoadStorage;
            this.settings = settings;
            this.dataSavier = dataSavier;
            this.workersManager = new WorkersManager(workerSettings.Value);
            this.allowedLanguages = languagesSettings.Value.AllowedLanguages;
        }

        public async Task Start(RunningMode mode = RunningMode.SingleThread)
        {
            this.toLoadStorage.AddToLoads(settings.StartUrls);
            if (mode == RunningMode.SingleThread)
                await this.StartProcessing();
            else
                StartMultitaskingProcessing();
        }

        private readonly short toSaveNumber = 100;  //кількість оброблених файлів для повторного зберження під час роботи в однопоточному режимі
        private async Task StartProcessing()
        {
            try
            {
                while (this.toLoadStorage.GetToLoadCount() > 0)
                {
                    if (this.toLoadStorage.TryGetNext(out ToLoad toProcess))
                    {
                        await this.ProcessPage(toProcess!);
                        Console.WriteLine($"{this.pageCounter++} ---- {toProcess!.Link}");
                        if (this.pageCounter % toSaveNumber == 0)
                        {
                            this.dataSavier.Save();
                        }
                        Thread.Sleep(delay);
                    }
                }
            }
            catch (Exception ex)
            {
                this.dataSavier.Save();
                Console.WriteLine(ex.Message);
            }
        }
        
        private void StartMultitaskingProcessing()
        {
            try
            {
                RunNewWorkers();
            }
            catch (Exception ex)
            {
                this.dataSavier.Save();
                Console.WriteLine(ex.Message);
            }
        }

        private void RunNewWorkers()
        {
            lock (lockObject)
            {
                var availableWorkers = this.workersManager.GetAvailableWorkersNumber();
                if (availableWorkers <= 0) return;

                var newDomains = this.toLoadStorage.GetUniqueDomains()
                    .Where(x => !this.domainsTakenByWorkers.Contains(x))
                    .Take(availableWorkers)
                    .ToList();
                foreach (var domain in newDomains)
                {
                    if (!this.workersManager.TryRunNewWroker(async () => await ProcessDomain(domain), RunNewWorkers))
                        break;
                    domainsTakenByWorkers.Add(domain);
                    Console.WriteLine($"taken domains count ---- {this.domainsTakenByWorkers.Count}");
                }
            }
        }

        private async Task ProcessDomain(string domain)
        {
            while (this.toLoadStorage.TryGetNextByDomain(out var toLoad, domain))
            {
                await ProcessPage(toLoad);
                RunNewWorkers();
                Thread.Sleep(delay);
            }
            this.dataSavier.Save();
            this.toLoadStorage.AddIgnoredDomain(domain);
            this.sitesStorage.RemoveSiteByDomain(domain);
        }

        private async Task ProcessPage(ToLoad toLoad)
        {
            var page = await this.loader.LoadPage(toLoad.Link);
            if (page.Length == 0) return;
            ParsedPage parsedPage = await this.parser.Parse(page, toLoad.BaseUrl);
            if (this.allowedLanguages.Contains(parsedPage.Language))
            {
                this.sitesStorage.StorePage(parsedPage, toLoad);
                this.toLoadStorage.AddToLoads(parsedPage.Links, toLoad.Domain, toLoad.ParentshipDepth ?? settings.ParentshipDepth);
            }
            else this.sitesStorage.StorePage(null, toLoad);
            Console.WriteLine($"Parsed ---- {toLoad.Link} ---- {DateTime.Now.ToString("ss:mm:hh")}");
        }

    }
}