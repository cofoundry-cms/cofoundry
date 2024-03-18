﻿namespace Cofoundry.Domain;

/// <summary>
/// Cache for page directories, which are frequently requested to 
/// work out routing
/// </summary>
public interface IPageDirectoryCache
{
    /// <summary>
    /// Gets a collection of directory routes, if the collection is already cached it 
    /// is returned, otherwise the getter is invoked and the result is cached and returned
    /// </summary>
    /// <param name="getter">Function to invoke if the entities are not in the cache</param>
    IReadOnlyCollection<PageDirectoryRoute> GetOrAdd(Func<IReadOnlyCollection<PageDirectoryRoute>> getter);

    /// <summary>
    /// Clears all items in the page directory cache
    /// </summary>
    void Clear();
}
