
using SitesGatherer.Extensions;
using SitesGatherer.Sevices.SitesStorageService;
using SitesGatherer.Sevices.ToLoadStorageService.Models;

namespace SitesGatherer.Sevices.ToLoadStorageService
{
    public class ToLoadStorage : IToLoadStorage
    {
        public readonly Queue<ToLoad> toLoads = [];
        private readonly Dictionary<int, HashSet<string>> hashLookup = [];

        private readonly ISitesStorage parsedStorage;
        private readonly ISitesStorage skippedStorage;
        public ToLoadStorage(ISitesStorage parsedStorage, ISitesStorage skippedStorage)
        {
            this.parsedStorage = parsedStorage;
            this.skippedStorage = skippedStorage;
        }

        public ToLoad? GetNext()
        {
            if (toLoads.Count == 0)
                return null;

            var next = toLoads.Dequeue();
            int hash = next.LinkHash;

            if (hashLookup.TryGetValue(hash, out var set))
            {
                set.Remove(next.Link);

                if (set.Count == 0)
                    hashLookup.Remove(hash);
            }
            return next;
        }

        public bool TryGetNext(out ToLoad toLoad)
        {
            if (toLoads.Count == 0)
            {
                toLoad = new ToLoad();
                return false;
            }

            toLoad = toLoads.Dequeue();
            int hash = toLoad.LinkHash;

            if (hashLookup.TryGetValue(hash, out var set))
            {
                set.Remove(toLoad.Link);

                if (set.Count == 0)
                    hashLookup.Remove(hash);
            }
            return true;
        }

        private void AddToReferenceList(IEnumerable<ToLoad> newItems)
        {
            foreach (var item in newItems)
            {
                int hash = item.LinkHash;

                if (!hashLookup.TryGetValue(hash, out var set))
                {
                    set = new HashSet<string>();
                    hashLookup[hash] = set;
                }

                set.Add(item.Link);
                toLoads.Enqueue(item);
            }
        }

        public IEnumerable<ToLoad> AddToLoads(List<string> urls, string? parentDomain = null, int? parentshipDepth = null)
        {
            var newToLoad = urls.GetToLoads(parentDomain, parentshipDepth);

            if (parentshipDepth == 0 && parentDomain != null)
            {
                newToLoad = newToLoad.Where(x => x.Domain == parentDomain);
            }
            
            //filter out duplicates
            newToLoad = newToLoad.DistinctBy(x => x.LinkHash);

            //filter out already processed links
            newToLoad = newToLoad
            .Where(x => !this.parsedStorage.Contains(x.Domain, x.PathParts)
                && !this.skippedStorage.Contains(x.Domain, x.PathParts));

            //filter out already added
            newToLoad = newToLoad
            .Where(item =>
            {
                int hash = item.LinkHash;
                return !hashLookup.TryGetValue(hash, out var set) || !set.Contains(item.Link);
            });

            //adding new links
            AddToReferenceList(newToLoad);
            return newToLoad;
        }

        public int GetToLoadCount() => this.toLoads.Count;
    }
}