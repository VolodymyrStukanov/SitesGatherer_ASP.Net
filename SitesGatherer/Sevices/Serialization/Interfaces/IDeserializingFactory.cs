
namespace SitesGatherer.Sevices.Serialization.Interfaces
{
    public interface IDeserializingFactory<out T, in U>
    {
        T FromDto(U dto);
    }
}