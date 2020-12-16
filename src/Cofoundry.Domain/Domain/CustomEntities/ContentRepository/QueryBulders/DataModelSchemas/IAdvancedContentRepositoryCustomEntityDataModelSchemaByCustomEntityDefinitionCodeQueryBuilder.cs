using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries to extract and return meta data information about a custom 
    /// entity data model for a specific custom entity definition.
    /// </summary>
    public interface IAdvancedContentRepositoryCustomEntityDataModelSchemaByCustomEntityDefinitionCodeQueryBuilder
    {
        /// <summary>
        /// Extracts and return meta data information about a custom 
        /// entity data model for a specific custom entity definition.
        /// </summary>
        IDomainRepositoryQueryContext<CustomEntityDataModelSchema> AsDetails();
    }
}
