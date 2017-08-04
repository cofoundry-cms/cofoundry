using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class CustomEntityVersionPageBlockEntityDefinition : IDependableEntityDefinition
    {
        public static string DefinitionCode = "COFCEB";

        public string EntityDefinitionCode { get { return DefinitionCode; } }

        public string Name { get { return "Custom Entity Version Page Block"; } }

        public IQuery<IDictionary<int, RootEntityMicroSummary>> CreateGetEntityMicroSummariesByIdRangeQuery(IEnumerable<int> ids)
        {
            return new GetCustomEntityVersionPageBlockEntityMicroSummariesByIdRangeQuery(ids);
        }
    }
}
