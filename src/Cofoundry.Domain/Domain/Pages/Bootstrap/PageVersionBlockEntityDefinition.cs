using Cofoundry.Domain.CQS;
using System.Collections.Generic;

namespace Cofoundry.Domain
{
    public class PageVersionBlockEntityDefinition : IDependableEntityDefinition
    {
        public const string DefinitionCode = "COFPGB";

        public string EntityDefinitionCode => DefinitionCode;

        public string Name => "Page Version Block";

        public IQuery<IDictionary<int, RootEntityMicroSummary>> CreateGetEntityMicroSummariesByIdRangeQuery(IEnumerable<int> ids)
        {
            return new GetPageVersionBlockEntityMicroSummariesByIdRangeQuery(ids);
        }
    }
}
