using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to PageVersionSummary objects.
    /// </summary>
    public interface IPageVersionSummaryMapper
    {
        /// <summary>
        /// Maps a set of paged EF PageVersion records for a single page into 
        /// PageVersionSummary objects.
        /// </summary>
        /// <param name="pageId">Id of the page that these versions belong to.</param>
        /// <param name="dbResult">Paged result set of records to map.</param>
        PagedQueryResult<PageVersionSummary> MapVersions(int pageId, PagedQueryResult<PageVersion> dbResult);
    }
}
