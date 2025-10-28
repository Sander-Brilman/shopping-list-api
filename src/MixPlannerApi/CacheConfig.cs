using System;
using Microsoft.Extensions.Caching.Memory;

namespace MixPlannerApi;

public static class CacheConfig
{
    public enum Keys
    {
        AllShoppingLists,

    }

    public static readonly MemoryCacheEntryOptions options = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
    };
    
}

