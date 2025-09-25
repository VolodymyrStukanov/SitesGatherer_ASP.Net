namespace SitesGatherer.Sevices.PagesHandler
{
    public interface IPagesHandler
    {
        public Task Start(RunnigMode mode = RunnigMode.SingleThread);
    }
}