using Microsoft.AspNetCore.Http;

namespace Cofoundry.Web;

/// <summary>
/// Default implementation of <see cref="IVisualEditorStateCache"/>.
/// </summary>
public class VisualEditorStateCache : IVisualEditorStateCache
{
    const string CACHE_KEY = "Cofoundry.Web.VisualEditorStateCache";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public VisualEditorStateCache(
        IHttpContextAccessor httpContextAccessor
        )
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc/>
    public VisualEditorState? Get()
    {
        var cache = _httpContextAccessor.HttpContext?.Items;
        if (cache == null)
        {
            return null;
        }

        return cache[CACHE_KEY] as VisualEditorState;
    }

    /// <inheritdoc/>
    public void Set(VisualEditorState data)
    {
        var cache = _httpContextAccessor.HttpContext?.Items;

        if (cache == null)
        {
            throw new InvalidOperationException("Cannot set the cache outside of a request.");
        }

        cache[CACHE_KEY] = data;
    }
}
