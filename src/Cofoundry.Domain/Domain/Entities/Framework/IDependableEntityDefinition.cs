using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Indicates that an entity can be a dependecy to other entities.
    /// </summary>
    public interface IDependableEntityDefinition : IEntityDefinition
    {
        /// <summary>
        /// Returns a query that will get information about the aggregate root associated with entity
        /// ids provided. I.e. for an Asset the aggregate root is an Asset, but for a PageVersion, the 
        /// aggregate root is the page.
        /// </summary>
        /// <param name="ids">Ids of the entities to get</param>
        IQuery<IDictionary<int, RootEntityMicroSummary>> CreateGetEntityMicroSummariesByIdRangeQuery(IEnumerable<int> ids);
    }
}
