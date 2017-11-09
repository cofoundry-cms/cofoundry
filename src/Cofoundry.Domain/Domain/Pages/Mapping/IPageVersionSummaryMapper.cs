using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple mapper for mapping to PageVersionSummary objects.
    /// </summary>
    public interface IPageVersionSummaryMapper
    {
        /// <summary>
        /// Maps a collection EF PageVersion records for a single page into 
        /// a collection of PageVersionSummary objects.
        /// </summary>
        /// <param name="pageId">Id of the page that these versions belong to.</param>
        /// <param name="dbVersions">PageVersion records from the database to map.</param>
        List<PageVersionSummary> MapVersions(int pageId, ICollection<PageVersion> dbVersions);
    }
}
