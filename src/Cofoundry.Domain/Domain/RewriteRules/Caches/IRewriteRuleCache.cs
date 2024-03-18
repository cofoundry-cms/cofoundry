﻿namespace Cofoundry.Domain;

/// <summary>
/// Cache for rewrite rules, which are frequently requested to 
/// work out routing
/// </summary>
public interface IRewriteRuleCache
{
    /// <summary>
    /// Gets a collection of rewrite rules, if the collection is already cached it 
    /// is returned, otherwise the getter is invoked and the result is cached and returned
    /// </summary>
    /// <param name="getter">Function to invoke if the rewrite rules are not in the cache</param>
    Task<IReadOnlyCollection<RewriteRuleSummary>> GetOrAddAsync(Func<Task<IReadOnlyCollection<RewriteRuleSummary>>> getter);

    /// <summary>
    /// Clears all items in the rewrite rules cache
    /// </summary>
    void Clear();
}
