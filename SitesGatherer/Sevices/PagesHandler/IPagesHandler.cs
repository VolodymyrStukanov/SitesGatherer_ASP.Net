using SitesGatherer.Sevices.PagesHandler.models;

namespace SitesGatherer.Sevices.PagesHandler
{
    public interface IPagesHandler
    {
        public Task Start(RunningMode mode = RunningMode.SingleThread);
    }
}