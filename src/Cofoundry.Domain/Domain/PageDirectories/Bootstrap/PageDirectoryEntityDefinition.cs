using Cofoundry.Domain.CQS;
using System.Collections.Generic;

namespace Cofoundry.Domain
{
    public sealed class PageDirectoryEntityDefinition : IDependableEntityDefinition
    {
        public const string DefinitionCode = "COFDIR";

        public string EntityDefinitionCode => DefinitionCode;

        public string Name => "Page Directory";

        public IQuery<IDictionary<int, RootEntityMicroSummary>> CreateGetEntityMicroSummariesByIdRangeQuery(IEnumerable<int> ids)
        {
            return new GetPageDirectoryEntityMicroSummariesByIdRangeQuery(ids);
        }
    }
}