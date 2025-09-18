
namespace SitesGatherer.Sevices.Assets
{
    public static class Locations
    {
        public static string BaseDirectory { get; } = $@"{Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName}\data";
        public static string ProcessedPath { get; } = $@"{Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName}\data\processed";
        public static string ProcessedFile { get; } = $@"processed_storage.json";
        public static string SkippedPath { get; } = $@"{Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName}\data\skipped";
        public static string SkippedFile { get; } = $@"skipped_storage.json";
        public static string ToLoadStroragePath { get; } = $@"{Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName}\data\to_load";
        public static string ToLoadFile { get; } = $@"to_load_storage.json";
        
    }
}