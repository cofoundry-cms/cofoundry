using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public class GetCustomEntityDataModelSchemaDetailsByDefinitionCodeQuery : IQuery<CustomEntityDataModelSchema>
    {
        public GetCustomEntityDataModelSchemaDetailsByDefinitionCodeQuery()
        {
        }

        public GetCustomEntityDataModelSchemaDetailsByDefinitionCodeQuery(string customEntityDefinitionCode)
        {
            CustomEntityDefinitionCode = customEntityDefinitionCode;
        }

        public string CustomEntityDefinitionCode { get; set; }
    }
}
