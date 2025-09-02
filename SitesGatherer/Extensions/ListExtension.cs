
namespace SitesGatherer.Extensions
{
    public static class ListExtension
    {

        public static List<T> AddRange<T>(this List<T> list, IEnumerable<T> collection)
        {
            list.AddRange(collection);
            return list;
        }

        public static HashSet<T> UnionWith<T>(this HashSet<T> list, HashSet<T> collection)
        {
            list.UnionWith(collection);
            return list;
        }
    }
}