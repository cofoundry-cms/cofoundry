using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries to extract and return meta data information about a custom 
    /// entity data model for a range of custom entity definitions.
    /// </summary>
    public interface IAdvancedContentRepositoryCustomEntityDataModelSchemaByCustomEntityDefinitionCodeRangeQueryBuilder
    {
        /// <summary>
        /// Query to extract and return meta data information about a custom 
        /// entity data model for a range of custom entity definitions.
        /// </summary>
        IDomainRepositoryQueryContext<IDictionary<string, CustomEntityDataModelSchema>> AsDetails();
    }
}
