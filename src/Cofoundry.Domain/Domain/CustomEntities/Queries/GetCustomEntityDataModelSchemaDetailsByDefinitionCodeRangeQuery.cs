using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Query to extract and return meta data information about a custom 
    /// entity data model for a range of custom entity definitions.
    /// </summary>
    public class GetCustomEntityDataModelSchemaDetailsByDefinitionCodeRangeQuery : IQuery<IDictionary<string, CustomEntityDataModelSchema>>
    {
        /// <summary>
        /// Query to extract and return meta data information about a custom 
        /// entity data model for a range of custom entity definitions.
        /// </summary>
        public GetCustomEntityDataModelSchemaDetailsByDefinitionCodeRangeQuery()
        {
        }

        /// <summary>
        /// Query to extract and return meta data information about a custom 
        /// entity data model for a range of custom entity definitions.
        /// </summary>
        /// <param name="customEntityDefinitionCodes">Range of definition codes to query (the unique 6 letter code representing the entity).</param>
        public GetCustomEntityDataModelSchemaDetailsByDefinitionCodeRangeQuery(
            IEnumerable<string> customEntityDefinitionCodes
            )
            : this(customEntityDefinitionCodes?.ToList())
        {
        }

        /// <summary>
        /// Query to extract and return meta data information about a custom 
        /// entity data model for a range of custom entity definitions.
        /// </summary>
        /// <param name="customEntityDefinitioCodes">Range of definition codes to query (the unique 6 letter code representing the entity).</param>
        public GetCustomEntityDataModelSchemaDetailsByDefinitionCodeRangeQuery(
            IReadOnlyCollection<string> customEntityDefinitionCodes
            )
        {
            if (customEntityDefinitionCodes == null) throw new ArgumentNullException(nameof(customEntityDefinitionCodes));

            CustomEntityDefinitionCodes = customEntityDefinitionCodes;
        }

        /// <summary>
        /// Range of definition codes to query (the unique 6 letter code representing the entity).
        /// </summary>
        [Required]
        public IReadOnlyCollection<string> CustomEntityDefinitionCodes { get; set; }
    }
}
