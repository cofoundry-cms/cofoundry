using Cofoundry.Domain.CQS;
using System.Collections.Generic;

namespace Cofoundry.Domain
{
    public class CustomEntityVersionEntityDefinition : IDependableEntityDefinition
    {
        public static string DefinitionCode = "COFCEV";

        public string EntityDefinitionCode => DefinitionCode;

        public string Name => "Custom Entity Version";

        public IQuery<IDictionary<int, RootEntityMicroSummary>> CreateGetEntityMicroSummariesByIdRangeQuery(IEnumerable<int> ids)
        {
            return new GetCustomEntityVersionEntityMicroSummariesByIdRangeQuery(ids);
        }
    }
}
