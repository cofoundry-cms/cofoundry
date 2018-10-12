using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class PageDirectoryEntityDefinition : IDependableEntityDefinition
    {
        public const string DefinitionCode = "COFDIR";

        public string EntityDefinitionCode { get { return DefinitionCode; } }

        public string Name { get { return "Page Directory"; } }

        public IQuery<IDictionary<int, RootEntityMicroSummary>> CreateGetEntityMicroSummariesByIdRangeQuery(IEnumerable<int> ids)
        {
            return new GetPageDirectoryEntityMicroSummariesByIdRangeQuery(ids);
        }
    }
}
