using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public class GetCustomEntityDefinitionSummaryByCodeQuery : IQuery<CustomEntityDefinitionSummary>
    {
        public GetCustomEntityDefinitionSummaryByCodeQuery()
        {
        }

        public GetCustomEntityDefinitionSummaryByCodeQuery(string customEntityDefinitionCode)
        {
            CustomEntityDefinitionCode = customEntityDefinitionCode;
        }

        public string CustomEntityDefinitionCode { get; set; }
    }
}
