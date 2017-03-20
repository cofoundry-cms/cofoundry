using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class WebDirectoryEntityDefinition : IDependableEntityDefinition
    {
        public static string DefinitionCode = "COFDIR";

        public string EntityDefinitionCode { get { return DefinitionCode; } }

        public string Name { get { return "Web Directory"; } }

        public IQuery<IDictionary<int, RootEntityMicroSummary>> CreateGetEntityMicroSummariesByIdRangeQuery(IEnumerable<int> ids)
        {
            return new GetWebDirectoryEntityMicroSummariesByIdRangeQuery(ids);
        }
    }
}
