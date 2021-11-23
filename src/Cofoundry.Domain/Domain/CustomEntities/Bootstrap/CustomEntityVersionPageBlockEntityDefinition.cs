using Cofoundry.Domain.CQS;
using System.Collections.Generic;

namespace Cofoundry.Domain
{
    public class CustomEntityVersionPageBlockEntityDefinition : IDependableEntityDefinition
    {
        public static string DefinitionCode = "COFCEB";

        public string EntityDefinitionCode => DefinitionCode;

        public string Name => "Custom Entity Version Page Block";

        public IQuery<IDictionary<int, RootEntityMicroSummary>> CreateGetEntityMicroSummariesByIdRangeQuery(IEnumerable<int> ids)
        {
            return new GetCustomEntityVersionPageBlockEntityMicroSummariesByIdRangeQuery(ids);
        }
    }
}
