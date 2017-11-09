using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple mapper for mapping to CustomEntityVersionSummary objects.
    /// </summary>
    public interface ICustomEntityVersionSummaryMapper
    {
        /// <summary>
        /// Maps a collection EF CustomEntityVersion records for a single custom entity into 
        /// a collection of CustomEntityVersionSummary objects. If the db record is null 
        /// then null is returned.
        /// </summary>
        /// <param name="customEntityId">Id of the custom entity that these versions belong to.</param>
        /// <param name="dbVersions">CustomEntityVersion records from the database to map.</param>
        List<CustomEntityVersionSummary> MapVersions(int customEntityId, ICollection<CustomEntityVersion> dbVersions);
    }
}
