using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retrieving a custom entity definition by it's unique 6 character code.
    /// </summary>
    public interface IContentRepositoryCustomEntityDefinitionByCodeQueryBuilder
    {
        /// <summary>
        /// Gets a custom entity definition by it's unique 6 character code.
        /// The returned projection contains much of the same data as the source 
        /// defintion class, but the main difference is that instead of using generics 
        /// to identify the data model type, there is instead a DataModelType property.
        /// </summary>
        IDomainRepositoryQueryContext<CustomEntityDefinitionSummary> AsSummary();

        /// <summary>
        /// Gets a custom entity definition by it's unique 6 character code.
        /// The returned object is a lightweight projection of the data defined in a custom entity 
        /// definition class. This is typically used as part of another domain model or
        /// for querying lists of definitions in the admin panel.
        /// </summary>
        IDomainRepositoryQueryContext<CustomEntityDefinitionMicroSummary> AsMicroSummary();
    }
}
