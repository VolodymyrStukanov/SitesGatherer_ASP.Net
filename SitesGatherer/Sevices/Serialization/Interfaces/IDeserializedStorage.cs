
namespace SitesGatherer.Sevices.Serialization.Interfaces
{
    public interface IDeserializedStorage<T>
    {
        void Restore(T data);
    }
}