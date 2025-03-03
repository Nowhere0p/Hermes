using Microsoft.Extensions.Caching.Memory;

namespace Hermes.Common;

public class HermesConfigManager(IMemoryCache cache) : IConfigManager
{
    private readonly IMemoryCache _cache = cache;
    private static string CACHE_CONFIG_NAME = "cachedConfiguration";
    private static int CACHE_RETENTION_IN_MINUTES = 10;

    public async Task<HermesConfiguration> GetConfigurationAsync()
    {
        return await _cache.GetOrCreateAsync(CACHE_CONFIG_NAME, async entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(CACHE_RETENTION_IN_MINUTES);
            return DummyResponseBuilder.DeserializeFromFile<HermesConfiguration>("HermesConfigs/hermesConfiguration.json");
        });
        
    }
}