using Assessment.Api.Entity;
using Microsoft.Extensions.Caching.Memory;

namespace Assessment.Api.Repository
{
    public class PersonCacheRepository : ICacheRepository<Person>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _memoryOptions;
        public PersonCacheRepository(IMemoryCache cache)
        {
            _memoryCache = cache;
            _memoryOptions = new MemoryCacheEntryOptions()
                  .SetSlidingExpiration(TimeSpan.FromSeconds(60))
                  .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                  .SetPriority(CacheItemPriority.Normal)
                  .SetSize(1024);
        }

        public List<Person>? TryGetList(string key)
        {
            return _memoryCache.TryGetValue(key, out List<Person>? result) ? result : null;
        }

        public void AddEntityToList(string key, Person value)
        {
            List<Person>? cachedEntity = TryGetList(key) ?? new List<Person>();
            cachedEntity.Add(value);
            UpdateCache(key, cachedEntity);
        }

        public void RemoveFromList(string key, int Id)
        {
            List<Person>? cachedEntity = TryGetList(key);
            if (cachedEntity != null)
            {
                Person? entityExist = cachedEntity.FirstOrDefault(c => c.Id ==Id);
                if(entityExist!=null)
                    cachedEntity.Remove(entityExist);
            }
            if (cachedEntity == null || cachedEntity.Any())
            {
                RemoveKey(key);
            }
            else
            {
                UpdateCache(key, cachedEntity);
            }
        }

        public void UpdateEntityInList(string key, int Id, Person value)
        {
            List<Person>? cachedEntity = TryGetList(key);
            if (cachedEntity != null)
            {
                Person? entityExist = cachedEntity.FirstOrDefault(c => c.Id == Id);
                if (entityExist != null)
                {
                    entityExist.Name = value.Name;
                    entityExist.Address = value.Address;
                }
            }
        }

        public void UpdateCache(string key, List<Person> value)
        {
            _memoryCache.Set(key, value, _memoryOptions);
        }

        public void RemoveKey(string key)
        {
            _memoryCache.Remove(key);
        }

    }
}
