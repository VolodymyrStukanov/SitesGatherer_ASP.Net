
namespace SitesGatherer.Extensions
{
    public static class HashSetExtension
    {

        public static HashSet<T> AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> collection)
        {
            foreach (var item in collection) {
                hashSet.Add(item);                
            }
            return hashSet;
        }
    }
}