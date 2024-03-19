using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public interface IPageRenderSummaryMapper
{
    /// <summary>
    /// Generic mapper for the PageRenderSummary projection.
    /// </summary>
    /// <param name="dbPageVersion">PageVersion record from the database. Must include the OpenGraphImageAsset property.</param>
    /// <param name="pageRoute">The page route to map to the new object.</param>
    PageRenderSummary Map(PageVersion dbPageVersion, PageRoute pageRoute);

    /// <summary>
    /// Generic mapper for the PageRenderSummary projection.
    /// </summary>
    /// <param name="dbPageVersion">PageVersion record from the database. Must include the OpenGraphImageAsset property.</param>
    /// <param name="pageRouteLookup">Dictionary containing all page routes.</param>
    PageRenderSummary Map(PageVersion dbPageVersion, IReadOnlyDictionary<int, PageRoute> pageRouteLookup);
}
