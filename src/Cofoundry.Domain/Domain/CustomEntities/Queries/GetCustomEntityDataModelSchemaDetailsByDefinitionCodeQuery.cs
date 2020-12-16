using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Query to extract and return meta data information about a custom 
    /// entity data model for a specific custom entity definition.
    /// </summary>
    public class GetCustomEntityDataModelSchemaDetailsByDefinitionCodeQuery : IQuery<CustomEntityDataModelSchema>
    {
        /// <summary>
        /// Query to extract and return meta data information about a custom 
        /// entity data model for a specific custom entity definition.
        /// </summary>
        public GetCustomEntityDataModelSchemaDetailsByDefinitionCodeQuery()
        {
        }

        /// <summary>
        /// Query to extract and return meta data information about a custom 
        /// entity data model for a specific custom entity definition.
        /// </summary>
        /// <param name="customEntityDefinitionCode">Unique 6 letter code representing the entity.</param>
        public GetCustomEntityDataModelSchemaDetailsByDefinitionCodeQuery(string customEntityDefinitionCode)
        {
            CustomEntityDefinitionCode = customEntityDefinitionCode;
        }

        /// <summary>
        /// Unique 6 letter code representing the entity.
        /// </summary>
        public string CustomEntityDefinitionCode { get; set; }
    }
}
