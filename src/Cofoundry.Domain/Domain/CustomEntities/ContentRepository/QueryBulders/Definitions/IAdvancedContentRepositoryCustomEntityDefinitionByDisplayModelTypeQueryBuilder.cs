using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for getting the custom entity definition associated with a
    /// concrete ICustomEntityDisplayModel type.
    /// </summary>
    public interface IAdvancedContentRepositoryCustomEntityDefinitionByDisplayModelTypeQueryBuilder
    {
        /// <summary>
        /// Query to get a custom entity definition by display model type definition.
        /// The returned object is a lightweight projection of the data defined in a custom entity 
        /// definition class and is typically used as part of another domain model.
        /// </summary>
        IDomainRepositoryQueryContext<CustomEntityDefinitionMicroSummary> AsMicroSummary();
    }
}
