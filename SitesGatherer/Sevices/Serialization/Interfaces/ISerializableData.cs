
namespace SitesGatherer.Sevices.Serialization.Interfaces
{
    public interface ISerializableData<out T>
    {
        T ToDto();
    }

}