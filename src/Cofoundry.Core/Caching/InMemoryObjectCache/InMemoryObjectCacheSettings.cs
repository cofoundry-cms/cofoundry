using Cofoundry.Core.Configuration;

namespace Cofoundry.Core.Caching;

/// <summary>
/// Settings specificaly for the default in-memory object cache implementation.
/// </summary>
public class InMemoryObjectCacheSettings : CofoundryConfigurationSettingsBase
{
    /// <summary>
    /// The cache mode that should be used to determine the lifetime
    /// of data stored in the cache. Defaults to InMemoryObjectCacheMode.Persistent,
    /// which is the preferred mode for a single-server deployment. 
    /// InMemoryObjectCacheMode.PerScope can be used to enable a simple multi-server
    /// deployment.
    /// </summary>
    public InMemoryObjectCacheMode CacheMode { get; set; } = InMemoryObjectCacheMode.Persistent;
}
