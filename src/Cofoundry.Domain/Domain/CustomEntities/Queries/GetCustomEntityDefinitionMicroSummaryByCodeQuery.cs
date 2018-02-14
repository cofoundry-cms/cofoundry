using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public class GetCustomEntityDefinitionMicroSummaryByCodeQuery : IQuery<CustomEntityDefinitionMicroSummary>
    {
        public GetCustomEntityDefinitionMicroSummaryByCodeQuery()
        {
        }

        public GetCustomEntityDefinitionMicroSummaryByCodeQuery(string customEntityDefinitionCode)
        {
            CustomEntityDefinitionCode = customEntityDefinitionCode;
        }

        public string CustomEntityDefinitionCode { get; set; }
    }
}
