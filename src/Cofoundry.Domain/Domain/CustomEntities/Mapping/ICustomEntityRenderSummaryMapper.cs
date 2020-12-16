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
    /// Mapper designed to be used internally to map CustomEntityRenderSummary instances
    /// </summary>
    public interface ICustomEntityRenderSummaryMapper
    {
        /// <summary>
        /// Maps an EF CustomEntityVersion record from the db into a CustomEntityRenderSummary 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="dbResult">CustomEntityVersion record from the database.</param>
        /// <param name="executionContext">Context to run any sub queries under.</param>
        Task<CustomEntityRenderSummary> MapAsync(CustomEntityVersion dbResult, IExecutionContext executionContext);

        /// <summary>
        /// Maps a collection of EF CustomEntityVersion record from the db into CustomEntityRenderSummary 
        /// objects.
        /// </summary>
        /// <param name="dbResult">CustomEntityVersion record from the database.</param>
        /// <param name="executionContext">Context to run any sub queries under.</param>
        Task<ICollection<CustomEntityRenderSummary>> MapAsync(ICollection<CustomEntityVersion> dbResults, IExecutionContext executionContext);
    }
}
