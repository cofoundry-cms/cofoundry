using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetPageRoutesByCustomEntityDefinitionCodeQuery : IQuery<IEnumerable<PageRoute>>
    {
        public GetPageRoutesByCustomEntityDefinitionCodeQuery() { }

        public GetPageRoutesByCustomEntityDefinitionCodeQuery(string customEntityDefinitionCode)
        {
            CustomEntityDefinitionCode = customEntityDefinitionCode;
        }

        public string CustomEntityDefinitionCode { get; set; }
    }
}
