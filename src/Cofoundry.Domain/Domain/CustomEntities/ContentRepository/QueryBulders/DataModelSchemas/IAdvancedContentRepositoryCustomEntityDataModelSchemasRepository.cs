using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries and commands for custom entity data model schemas.
    /// </summary>
    public interface IAdvancedContentRepositoryCustomEntityDataModelSchemasRepository
    {
        #region queries

        /// <summary>
        /// Extracts and return meta data information about a custom 
        /// entity data model for a specific custom entity definition.
        /// </summary>
        /// <param name="customEntityDefinitionCode">Unique 6 letter code representing the entity.</param>
        IAdvancedContentRepositoryCustomEntityDataModelSchemaByCustomEntityDefinitionCodeQueryBuilder GetByCustomEntityDefinitionCode(string customEntityDefinitionCode);

        /// <summary>
        /// Extracts and return meta data information about a custom 
        /// entity data model for a range of custom entity definitions.
        /// </summary>
        /// <param name="customEntityDefinitionCodes">Range of definition codes to query (the unique 6 letter code representing the entity).</param>
        IAdvancedContentRepositoryCustomEntityDataModelSchemaByCustomEntityDefinitionCodeRangeQueryBuilder GetByCustomEntityDefinitionCodeRange(IEnumerable<string> customEntityDefinitionCodes);

        #endregion
    }
}
