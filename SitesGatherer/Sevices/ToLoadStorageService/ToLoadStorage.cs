using SitesGatherer.Extensions;
using SitesGatherer.Sevices.Serialization.ModelsDTO;
using SitesGatherer.Sevices.SitesStorageService.Interfaces;
using SitesGatherer.Sevices.ToLoadStorageService.Models;

namespace SitesGatherer.Sevices.ToLoadStorageService
{
    public class ToLoadStorage : IToLoadStorage
    {
        private readonly Lock lockObject = new();
        
        private readonly List<ToLoad> toLoads = [];
        private readonly Dictionary<int, HashSet<string>> hashLookup = [];
        private readonly IParsedStorage parsedStorage;
        private readonly ISkippedStorage skippedStorage;
        public ToLoadStorage(IParsedStorage parsedStorage, ISkippedStorage skippedStorage)
        {
            this.parsedStorage = parsedStorage;
            this.skippedStorage = skippedStorage;
        }

        public ToLoad? GetNext()
        {
            lock (lockObject)
            {
                if (toLoads.Count == 0)
                    return null;

                var next = toLoads.FirstOrDefault();
                if (next == null) return null;

                toLoads.Remove(next);
                int hash = next.LinkHash;

                if (hashLookup.TryGetValue(hash, out var set))
                {
                    set.Remove(next.Link);

                    if (set.Count == 0)
                        hashLookup.Remove(hash);
                }
                return next;                
            }
        }

        public bool TryGetNext(out ToLoad toLoad)
        {
            lock (lockObject)
            {
                var found = toLoads.FirstOrDefault();
                if (found == null)
                {
                    toLoad = new ToLoad();
                    return false;                    
                }
                toLoad = found;
                toLoads.Remove(toLoad);

                int hash = toLoad.LinkHash;

                if (hashLookup.TryGetValue(hash, out var set))
                {
                    set.Remove(toLoad.Link);

                    if (set.Count == 0)
                        hashLookup.Remove(hash);
                }
                return true;
            }
        }

        public bool TryGetNextByDomain(out ToLoad toLoad, string domain)
        {
            lock (lockObject)
            {
                var found = toLoads.Find(x => x.Domain == domain);
                if (found == null)
                {
                    toLoad = new ToLoad();
                    return false;
                }
                toLoad = found;
                toLoads.Remove(toLoad);

                int hash = toLoad.LinkHash;
                if (hashLookup.TryGetValue(hash, out var set))
                {
                    set.Remove(toLoad.Link);

                    if (set.Count == 0)
                        hashLookup.Remove(hash);
                }
                return true;
            }
        }

        private void AddToLinkList(IEnumerable<ToLoad> newItems)
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
                toLoads.Add(item);
            }
        }

        public void AddToLoads(List<string> urls, string? parentDomain = null, int? parentshipDepth = null)
        {

            lock (lockObject)
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
                AddToLinkList(newToLoad);
            }
        }

        public int GetToLoadCount() => this.toLoads.Count;

        public ToLoadStorageDto ToDto()
        {
            lock (lockObject)
            {
                return new ToLoadStorageDto
                {
                    ToLoadDtos = this.toLoads.Select(x => x.ToDto()),
                };
            }
        }

        public void Restore(ToLoadStorageDto data)
        {
            foreach (var toLoad in data.ToLoadDtos) {
                AddToLoads([toLoad.Link], toLoad.ParentDomain, toLoad.ParentshipDepth);
            }
        }

        public List<string> GetUniqueDomains()
        {
            lock (lockObject)
            {
                return this.toLoads.Aggregate(new List<string>(), (total, next) =>
                {
                    if (!total.Contains(next.Domain)) return [.. total, next.Domain];
                    return total;
                });
            }
        }
    }
}