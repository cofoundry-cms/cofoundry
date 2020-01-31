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
        IAdvancedContentRepositoryCustomEntityDataModelSchemaByCustomEntityDefinitionCodeQueryBuilder GetByCustomEntityDefinitionCode(string customEntityDefinitionCode);

        /// <summary>
        /// Extracts and return meta data information about a custom 
        /// entity data model for a range of custom entity definitions.
        /// </summary>
        IAdvancedContentRepositoryCustomEntityDataModelSchemaByCustomEntityDefinitionCodeRangeQueryBuilder GetByCustomEntityDefinitionCodeRange(IEnumerable<string> customEntityDefinitionCodes);

        #endregion
    }
}
