using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to CustomEntitySummary objects.
    /// </summary>
    public interface ICustomEntitySummaryMapper
    {
        /// <summary>
        /// Maps a collection of EF CustomEntityPublishStatusQuery records from the db 
        /// into CustomEntitySummary objects. The records must include data for the the 
        /// CustomEntity, CustomEntityVersion, CustomEntity.Creator and CustomEntityVersion.Creator 
        /// properties.
        /// </summary>
        /// <param name="dbCustomEntities">Collection of CustomEntityPublishStatusQuery records to map.</param>
        /// <param name="executionContext">Execution context to pass down when executing child queries.</param>
        Task<List<CustomEntitySummary>> MapAsync(ICollection<CustomEntityPublishStatusQuery> dbStatusQueries, IExecutionContext executionContext);
    }
}
