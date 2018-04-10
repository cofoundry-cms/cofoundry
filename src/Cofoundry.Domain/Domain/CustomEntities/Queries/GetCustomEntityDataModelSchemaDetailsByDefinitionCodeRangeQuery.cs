using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain
{
    public class GetCustomEntityDataModelSchemaDetailsByDefinitionCodeRangeQuery : IQuery<IDictionary<string, CustomEntityDataModelSchema>>
    {
        public GetCustomEntityDataModelSchemaDetailsByDefinitionCodeRangeQuery()
        {
        }

        public GetCustomEntityDataModelSchemaDetailsByDefinitionCodeRangeQuery(
            IEnumerable<string> customEntityIds
            )
            : this(customEntityIds?.ToList())
        {
        }

        public GetCustomEntityDataModelSchemaDetailsByDefinitionCodeRangeQuery(
            IReadOnlyCollection<string> customEntityDefinitionCodes
            )
        {
            if (customEntityDefinitionCodes == null) throw new ArgumentNullException(nameof(customEntityDefinitionCodes));

            CustomEntityDefinitionCodes = customEntityDefinitionCodes;
        }

        [Required]
        public IReadOnlyCollection<string> CustomEntityDefinitionCodes { get; set; }
    }
}
