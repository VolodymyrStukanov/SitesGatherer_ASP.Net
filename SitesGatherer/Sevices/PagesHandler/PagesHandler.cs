using SitesGatherer.Sevices.HTMLParser;
using SitesGatherer.Sevices.LoadService;
using SitesGatherer.Sevices.PagesHandler.Models;
using SitesGatherer.Sevices.SitesStorageService;
using SitesGatherer.Sevices.ToLoadStorageService.Models;
using SitesGatherer.Sevices.ToLoadStorageService;
using SitesGatherer.Sevices.HTMLParser.Models;
using Microsoft.Extensions.Options;
using SitesGatherer.Sevices.Settings;

namespace SitesGatherer.Sevices.PagesHandler
{
    public class PagesHandler : IPagesHandler
    {
        private readonly WorkerSettings workerSettings;
        
        private readonly ILoader loader;
        private readonly ISitesStorage parsedStorage;
        private readonly ISitesStorage skippedStorage;
        private readonly IToLoadStorage toLoadStorage;
        private readonly IHtmlParser parser;
        private readonly ISettingsService settings;

        public PagesHandler(
            ILoader loader,
            ISitesStorage parsedStorage,
            ISitesStorage skippedStorage,
            IHtmlParser parser,
            IOptions<WorkerSettings> workerSettings,
            ISettingsService settings,
            IToLoadStorage toLoadStorage)
        {
            this.loader = loader;
            this.parsedStorage = parsedStorage;
            this.skippedStorage = skippedStorage;
            this.parser = parser;
            this.workerSettings = workerSettings.Value;
            this.toLoadStorage = toLoadStorage;
            this.settings = settings;
        }

        public async Task Start()
        {   
            this.toLoadStorage.AddToLoads(settings.StartUrls);
            await this.StartProcessing();
        }

        private async Task StartProcessing()
        {
            int counter = 0;
            while (this.toLoadStorage.GetToLoadCount() > 0)
            {
                if (this.toLoadStorage.TryGetNext(out ToLoad toProcess))
                {
                    await this.ProcessPage(toProcess!);
                    Console.WriteLine($"{counter++} ---- {toProcess!.Link}");
                    Thread.Sleep(200);
                }
            }
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
            }
            else
            {
                this.skippedStorage.StorePage(parsedPage, toLoad);
            }
        }

    }
}