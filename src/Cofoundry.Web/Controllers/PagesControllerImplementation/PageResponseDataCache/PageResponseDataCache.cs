using Microsoft.AspNetCore.Http;

namespace Cofoundry.Web;

/// <summary>
/// Caches the data used to render dynamic pages for the duration of the 
/// request so it can be accessed outside of the controller and view. 
/// Primarily used to render the siteviewer via SiteViewerContentFilterAttribute.
/// </summary>
public class PageResponseDataCache : IPageResponseDataCache
{
    const string CACHE_KEY = "Cofoundry.Web.PageResponseDataCache";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PageResponseDataCache(
        IHttpContextAccessor httpContextAccessor
        )
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc/>
    public IPageResponseData? Get()
    {
        var cache = _httpContextAccessor.HttpContext?.Items;
        if (cache == null)
        {
            return null;
        };

        return cache[CACHE_KEY] as PageResponseData;
    }

    /// <inheritdoc/>
    public void Set(IPageResponseData data)
    {
        var cache = _httpContextAccessor.HttpContext?.Items;

        if (cache == null)
        {
            throw new InvalidOperationException("Cannot set the cache outside of a request.");
        }

        cache[CACHE_KEY] = data;
    }
}
