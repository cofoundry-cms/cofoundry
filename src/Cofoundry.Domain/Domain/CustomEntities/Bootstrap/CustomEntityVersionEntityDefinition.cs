using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class CustomEntityVersionEntityDefinition : IDependableEntityDefinition
    {
        public static string DefinitionCode = "COFCEV";

        public string EntityDefinitionCode { get { return DefinitionCode; } }

        public string Name { get { return "Custom Entity Version"; } }

        public IQuery<IDictionary<int, RootEntityMicroSummary>> CreateGetEntityMicroSummariesByIdRangeQuery(IEnumerable<int> ids)
        {
            return new GetCustomEntityVersionEntityMicroSummariesByIdRangeQuery(ids);
        }
    }
}
