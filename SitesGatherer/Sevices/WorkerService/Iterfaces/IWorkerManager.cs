namespace SitesGatherer.Sevices.WorkerService.Iterfaces
{
    public interface IWorkerManager
    {
        public bool TryRunNewWroker(Func<Task> action, Action? then = null);
        public int GetAvailableWorkersNumber();
    }
}