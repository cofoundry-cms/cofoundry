using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class PageVersionModuleEntityDefinition : IDependableEntityDefinition
    {
        public static string DefinitionCode = "COFPGM";

        public string EntityDefinitionCode { get { return DefinitionCode; } }

        public string Name { get { return "Page Version Module"; } }

        public IQuery<IDictionary<int, RootEntityMicroSummary>> CreateGetEntityMicroSummariesByIdRangeQuery(IEnumerable<int> ids)
        {
            return new GetPageVersionModuleEntityMicroSummariesByIdRangeQuery(ids);
        }
    }
}
