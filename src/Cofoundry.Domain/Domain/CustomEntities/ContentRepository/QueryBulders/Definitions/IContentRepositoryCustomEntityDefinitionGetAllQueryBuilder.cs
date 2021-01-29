using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retrieving all custom entity definitions.
    /// </summary>
    public interface IContentRepositoryCustomEntityDefinitionGetAllQueryBuilder
    {
        /// <summary>
        /// Gets a collection of all custom entity definitions registered
        /// with the system. The returned object is a lightweight projection of the
        /// data defined in a custom entity definition class which is typically used 
        /// as part of another domain model.
        /// </summary>
        IDomainRepositoryQueryContext<ICollection<CustomEntityDefinitionMicroSummary>> AsMicroSummaries();

        /// <summary>
        /// Gets a collection of all custom entity definitions registered
        /// with the system. The returned projections contain much of the same data 
        /// as the source defintion class, but the main difference is that instead of 
        /// using generics to identify the data model type, there is instead a 
        /// DataModelType property.
        /// </summary>
        IDomainRepositoryQueryContext<ICollection<CustomEntityDefinitionSummary>> AsSummaries();
    }
}
