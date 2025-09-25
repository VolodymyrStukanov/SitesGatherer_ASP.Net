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

namespace SitesGatherer.Sevices.PagesHandler
{
    public class PagesHandler : IPagesHandler
    {
        private readonly Lock lockObject = new();
        private readonly List<string> domainsTakenByWorkers = [];
        private readonly WorkersManager workersManager;
        
        private readonly ILoader loader;
        private readonly IParsedStorage parsedStorage;
        private readonly ISkippedStorage skippedStorage;
        private readonly IToLoadStorage toLoadStorage;
        private readonly IHtmlParser parser;
        private readonly ISettingsService settings;
        private readonly DataSavier dataSavier;
        private int pageCounter = 0;

        public PagesHandler(
            ILoader loader,
            IParsedStorage parsedStorage,
            ISkippedStorage skippedStorage,
            IOptions<WorkerSettings> workerSettings,
            ISettingsService settings,
            IToLoadStorage toLoadStorage,
            DataSavier dataSavier)
        {
            this.loader = loader;
            this.parsedStorage = parsedStorage;
            this.skippedStorage = skippedStorage;
            this.parser = new HtmlParser();
            this.toLoadStorage = toLoadStorage;
            this.settings = settings;
            this.dataSavier = dataSavier;
            this.workersManager = new WorkersManager(workerSettings.Value);
        }

        public async Task Start(RunnigMode mode = RunnigMode.SingleThread)
        {
            this.toLoadStorage.AddToLoads(settings.StartUrls);
            if (mode == RunnigMode.SingleThread)
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
                        Thread.Sleep(200);
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
            }
            this.dataSavier.Save();
        }

        private async Task ProcessPage(ToLoad toLoad)
        {
            var page = await this.loader.LoadPage(toLoad.Link);
            if (page.Length == 0) return;
            ParsedPage parsedPage = await this.parser.Parse(page, toLoad.BaseUrl);
            if (parsedPage.Language == Languages.UA)
            {
                this.parsedStorage.StorePage(parsedPage, toLoad);
                this.toLoadStorage.AddToLoads(parsedPage.Links, toLoad.Domain, toLoad.ParentshipDepth ?? settings.ParentshipDepth);
                Console.WriteLine($"Parsed ---- {toLoad.Link}");
            }
            else
            {
                this.skippedStorage.StorePage(parsedPage, toLoad);
                Console.WriteLine($"Skipped ---- {toLoad.Link}");
            }
        }

    }
}