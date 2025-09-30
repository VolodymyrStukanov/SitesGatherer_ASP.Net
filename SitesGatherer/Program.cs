
using SitesGatherer.Sevices.DataStorageService;
using SitesGatherer.Sevices.LeadsService;
using SitesGatherer.Sevices.LoadService;
using SitesGatherer.Sevices.PagesHandler;
using SitesGatherer.Sevices.PagesHandler.models;
using SitesGatherer.Sevices.Serialization.Extensions;
using SitesGatherer.Sevices.Settings;
using SitesGatherer.Sevices.SitesStorageService;
using SitesGatherer.Sevices.SitesStorageService.Interfaces;
using SitesGatherer.Sevices.ToLoadStorageService;
using SitesGatherer.Sevices.WorkerService.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient("LoaderClient", (serviseProvider, httpClient) =>
{
    httpClient.Timeout = TimeSpan.FromSeconds(30);

    // Simulate a modern Chrome browser on Windows
    httpClient.DefaultRequestHeaders.Add("User-Agent",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
        "AppleWebKit/537.36 (KHTML, like Gecko) " +
        "Chrome/115.0.0.0 Safari/537.36");

    // Tell the server we accept HTML and related formats
    httpClient.DefaultRequestHeaders.Add("Accept",
        "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");

    // Accept-Language is important for FB
    httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");

    // Looks like a browser navigating directly
    httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
    
    // // Keep the connection alive for performance (like browsers do)
    // httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");

    // Optional: mimic a browser's sec-fetch headers (some sites check them)
    httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");
    httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
    httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
    httpClient.DefaultRequestHeaders.Add("Sec-Fetch-User", "?1");

    // Optional but sometimes helps
    httpClient.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
});

#region services
builder.Services.Configure<WorkerSettings>(builder.Configuration.GetSection("WorkerSettings"));
builder.Services.Configure<LanguagesSettings>(builder.Configuration.GetSection("LanguagesSettings"));

builder.Services.AddSingleton<DataSavier>();
builder.Services.AddSingleton<ISettingsService, SettingsService>();
builder.Services.AddSingleton<ISitesStorage, SitesStorage>(options =>
{
    var settingsService = options.GetService<ISettingsService>()!;

    // string? json = null;
    string? json = settingsService.GetParsedStorageJSON();
    return json == null ? new SitesStorage() : json.DataStorageFromJson();
});
builder.Services.AddSingleton<IToLoadStorage, ToLoadStorage>(options =>
{
    var sitesStorage = options.GetService<ISitesStorage>()!;
    var settingsService = options.GetService<ISettingsService>()!;

    // string? json = null;
    string? json = settingsService.GetToLoadStorageJSON();
    return json == null ? new ToLoadStorage(sitesStorage) : json.ToLoadStorageFromJson(sitesStorage);
});
builder.Services.AddSingleton<IPagesHandler, PagesHandler>();

builder.Services.AddTransient<ILeadsGenerator, LeadsGenerator>();
builder.Services.AddTransient<ILoader>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var client = factory.CreateClient("LoaderClient");
    return new Loader(client);
});
#endregion services

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
