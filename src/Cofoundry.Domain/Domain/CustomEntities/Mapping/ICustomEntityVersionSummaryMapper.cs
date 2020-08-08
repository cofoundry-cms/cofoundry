using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to CustomEntityVersionSummary objects.
    /// </summary>
    public interface ICustomEntityVersionSummaryMapper
    {
        /// <summary>
        /// aps a set of paged EF CustomEntityVersion records for a single custom entity into 
        /// CustomEntityVersionSummary objects.
        /// </summary>
        /// <param name="customEntityId">Id of the custom entity that these versions belong to.</param>
        /// <param name="dbResult">Paged result set of records to map.</param>
        PagedQueryResult<CustomEntityVersionSummary> MapVersions(int customEntityId, PagedQueryResult<CustomEntityVersion> dbResult);
    }
}
