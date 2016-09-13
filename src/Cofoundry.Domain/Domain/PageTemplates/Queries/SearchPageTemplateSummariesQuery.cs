using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class SearchPageTemplateSummariesQuery : SimplePageableQuery, IQuery<PagedQueryResult<PageTemplateSummary>>
    {
        public string Name { get; set; }
    }
}
