using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class PageVersionEntityDefinition : IDependableEntityDefinition
    {
        public const string DefinitionCode = "COFPGV";

        public string EntityDefinitionCode { get { return DefinitionCode; } }

        public string Name { get { return "Page Version"; } }

        public IQuery<IDictionary<int, RootEntityMicroSummary>> CreateGetEntityMicroSummariesByIdRangeQuery(IEnumerable<int> ids)
        {
            return new GetPageVersionEntityMicroSummariesByIdRangeQuery(ids);
        }
    }
}
