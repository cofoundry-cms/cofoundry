using Cofoundry.Core.Caching;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IRewriteRuleCache"/>.
/// </summary>
public class RewriteRuleCache : IRewriteRuleCache
{
    private const string REWRITERULESUMMARY_CACHEKEY = "RewriteRuleSummaries";
    private const string CACHEKEY = "COF_RewriteRules";

    private readonly IObjectCache _cache;
    public RewriteRuleCache(IObjectCacheFactory cacheFactory)
    {
        _cache = cacheFactory.Get(CACHEKEY);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<RewriteRuleSummary>> GetOrAddAsync(Func<Task<IReadOnlyCollection<RewriteRuleSummary>>> getter)
    {
        var result = await _cache.GetOrAddAsync(REWRITERULESUMMARY_CACHEKEY, getter);

        if (result == null)
        {
            throw new InvalidOperationException($"Result of {nameof(_cache.GetOrAddAsync)} with key {REWRITERULESUMMARY_CACHEKEY} should never be null.");
        }

        return result;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _cache.Clear();
    }
}
