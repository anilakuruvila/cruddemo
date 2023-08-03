namespace Assessment.Api.Repository
{
    public interface ICacheRepository<T>
    {
        List<T>? TryGetList(string key);
        void AddEntityToList(string key, T value);
        void RemoveFromList(string key, int Id);
        void UpdateEntityInList(string key, int Id, T value);
        void UpdateCache(string key, List<T> value);
        void RemoveKey(string key);
    }
}
