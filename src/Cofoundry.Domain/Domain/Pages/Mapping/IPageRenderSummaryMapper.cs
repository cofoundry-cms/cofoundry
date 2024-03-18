﻿using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public interface IPageRenderSummaryMapper
{
    /// <summary>
    /// Genric mapper for objects that inherit from PageRenderSummary.
    /// </summary>
    /// <param name="dbPageVersion">PageVersion record from the database. Must include the OpenGraphImageAsset property.</param>
    /// <param name="pageRoute">The page route to map to the new object.</param>
    T Map<T>(PageVersion dbPageVersion, PageRoute pageRoute)
        where T : PageRenderSummary, new();

    /// <summary>
    /// Genric mapper for objects that inherit from PageRenderSummary.
    /// </summary>
    /// <param name="dbPageVersion">PageVersion record from the database. Must include the OpenGraphImageAsset property.</param>
    /// <param name="pageRouteLookup">Dictionary containing all page routes.</param>
    T Map<T>(PageVersion dbPageVersion, IReadOnlyDictionary<int, PageRoute> pageRouteLookup)
        where T : PageRenderSummary, new();
}
