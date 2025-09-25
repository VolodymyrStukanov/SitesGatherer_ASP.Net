using System.Collections.Concurrent;
using SitesGatherer.Sevices.WorkerService.Iterfaces;
using SitesGatherer.Sevices.WorkerService.Models;

namespace SitesGatherer.Sevices.WorkerService
{
    public class WorkersManager : IWorkerManager
    {
        private readonly int maxWorkersCount = 100;
        private readonly ConcurrentDictionary<int, Task> tasks = new ConcurrentDictionary<int, Task>();
        private readonly Lock lockObject = new();
        public WorkersManager(WorkerSettings workersSettings)
        {
            this.maxWorkersCount = workersSettings.MaxWorkersCount;
        }

        public bool TryRunNewWroker(Func<Task> action, Action? then = null)
        {
            lock (lockObject)
            {
                if (this.tasks.Count <= this.maxWorkersCount)
                {
                    var index = 0;
                    foreach (var i in this.tasks.Keys)
                    {
                        if (i - index > 1)
                        {
                            index = i - 1;
                            break;
                        }
                        index = i+1;
                    }
                    var task = Task.Run(async () =>
                    {
                        await action();
                    }).ContinueWith(x =>
                    {
                        this.tasks.Remove(index, out _);
                        Console.WriteLine($"Threads count ---- {this.tasks.Count}");
                        then?.Invoke();
                    });
                    this.tasks.TryAdd(index, task);
                    Console.WriteLine($"Threads count ---- {this.tasks.Count}");
                    return true;
                }
                return false;
            }
        }

        public int GetAvailableWorkersNumber()
        {
            lock (lockObject)
            {
                return this.maxWorkersCount - this.tasks.Count;
            }    
        }
    }
}