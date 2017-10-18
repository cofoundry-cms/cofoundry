using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple mapper for mapping to CustomEntityVersion objects.
    /// </summary>
    public interface ICustomEntitySummaryMapper
    {
        /// <summary>
        /// Maps a collection of EF CustomEntityVersion records from the db into a CustomEntitySummary 
        /// objects.
        /// </summary>
        /// <param name="dbVersions">Collection of vewrsions to map.</param>
        Task<List<CustomEntitySummary>> MapAsync(ICollection<CustomEntityVersion> dbVersions, IExecutionContext executionContext);
    }
}
