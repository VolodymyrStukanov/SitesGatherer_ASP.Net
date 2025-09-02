
namespace SitesGatherer.Extensions
{
    public static class StringExtension
    {
        public static string GetNumbers(this string str)
        {
            return string.Concat(str.Where(x => Int32.TryParse(x.ToString(), out int _)));
        }
        
    }
}