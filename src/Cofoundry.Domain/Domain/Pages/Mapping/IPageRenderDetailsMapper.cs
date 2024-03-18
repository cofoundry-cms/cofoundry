﻿using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public interface IPageRenderDetailsMapper
{
    /// <summary>
    /// Maps the main properties on a PageRenderDetails including
    /// page regions, but does not map the page block data.
    /// </summary>
    /// <param name="dbPageVersion">
    /// PageVersion record from the database. Must include the 
    /// OpenGraphImageAsset, PageTemplate and PageTemplate.PageTemplateRegions
    /// properties.
    /// </param>
    /// <param name="pageRouteLookup">
    /// Set of page routes to lookup the route property value.
    /// </param>
    PageRenderDetails Map(PageVersion dbPageVersion, IReadOnlyDictionary<int, PageRoute> pageRouteLookup);

    /// <summary>
    /// Maps the main properties on a PageRenderDetails including
    /// page regions, but does not map the page block data.
    /// </summary>
    /// <param name="dbPageVersion">
    /// PageVersion record from the database. Must include the 
    /// OpenGraphImageAsset, PageTemplate and PageTemplate.PageTemplateRegions
    /// properties.
    /// </param>
    /// <param name="pageRoute">
    /// The page route to map to the new object.
    /// </param>
    PageRenderDetails Map(PageVersion dbPageVersion, PageRoute pageRoute);
}
