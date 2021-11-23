using Cofoundry.Domain.CQS;
using System.Collections.Generic;

namespace Cofoundry.Domain
{
    public class PageEntityDefinition : IDependableEntityDefinition
    {
        public const string DefinitionCode = "COFPGE";

        public string EntityDefinitionCode => DefinitionCode;

        public string Name => "Page";

        public IQuery<IDictionary<int, RootEntityMicroSummary>> CreateGetEntityMicroSummariesByIdRangeQuery(IEnumerable<int> ids)
        {
            return new GetPageEntityMicroSummariesByIdRangeQuery(ids);
        }
    }
}
